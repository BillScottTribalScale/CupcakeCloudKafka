using System;
using System.Json;
using Microsoft.Extensions.Logging;
using Common.Lib.Kafka;


namespace CalculationEngine.Api.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class MessagePublisher : IMessagePublisher
    {
        private readonly ILogger<MessagePublisher> _logger;
        private readonly IKafkaPublisher _kafkaPublisher;
        private readonly IKafkaConfiguration _kafkaConfigHelper;
        private const string FILE_KEY = "fileKey";

        /// <summary>
        /// MessagePublisher Constructor with DI
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="kafkaConfighelper"></param>
        /// <param name="kafkaPublisher"></param>
        public MessagePublisher(ILogger<MessagePublisher> logger, IKafkaConfiguration kafkaConfighelper, IKafkaPublisher kafkaPublisher)
        {
            _logger = logger;
            _kafkaConfigHelper = kafkaConfighelper;
            _kafkaPublisher = kafkaPublisher;
        }

        /// <summary>
        /// This method is used to send the participant allocation message to kafka
        /// </summary>
        /// <param name="investment">participants investments</param>
        /// <returns>true/false</returns>
        public bool SendParticipantAllocationsMessage(string investment)
        {
            return sendMessage(investment);
        }

        private bool sendMessage(string investment)
        {
            try
            {
                Guid fileKey = getFileKey(investment);
                string topicName = _kafkaConfigHelper.ParticipantAllocationsTopicName;
                _kafkaPublisher.SendMessage(investment, topicName, fileKey);
                _logger.LogTrace("message sent to Kafka topicName={0}, fileKey={1}, payload={2}", topicName, fileKey, investment);
                return true;
            }
            catch (Exception e)
            {
                //TODO: WHAT DO WE DO WHEN AN ERROR POSTING TO KAFKA?
                _logger.LogError(e, e.Message);
            }
            return false;
        }

        private Guid getFileKey(string comp)
        {
            try
            {
                var objJsonContent = (JsonObject)JsonObject.Parse(comp);
                if (Guid.TryParse(objJsonContent[FILE_KEY], out Guid fileKey))
                    return fileKey;
            }
            catch (Exception ex)
            {
                //TODO: what will do if key is bad
                _logger.LogError("MessagePublisher:getFileKey - Error while creating a file key as guid.", ex);
            }
            return Guid.Empty;
        }
    }
}
