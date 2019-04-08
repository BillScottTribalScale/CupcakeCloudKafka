using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using FileProcessor.Api.Models;
using System.Json;
using System.Text;
using Common.Lib.Kafka;

namespace FileProcessor.Api.Services
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly ILogger<MessagePublisher> _logger;
        private readonly IKafkaPublisher _kafkaPublisher;
        private readonly IKafkaConfiguration _kafkaConfigHelper;

        public MessagePublisher(ILogger<MessagePublisher> logger, IKafkaConfiguration kafkaConfighelper, IKafkaPublisher kafkaPublisher)
        {
            _logger = logger;
            _kafkaConfigHelper = kafkaConfighelper;
            _kafkaPublisher = kafkaPublisher;
        }

        private bool sendMessages(List<CompensationInvestment> compList, string topicName)
        {
            foreach (CompensationInvestment comp in compList)
            {
                var jComp = comp.GenerateJson();
                try
                {
                    _kafkaPublisher.SendMessage(jComp, topicName, comp.fileKey);
                }
                catch (Exception e)
                {
                    _logger.LogError("MessagePublisher:sendMessage : Error while sending message to topic :{0}. Details :{1}", topicName, e);
                }
            }
            return true;
        }

        public void SendParticipantMessages(List<CompensationInvestment> investments)
        {
            sendMessages(investments, _kafkaConfigHelper.ParticipantTopicName);
        }
        public bool SendMetaMessages(JsonObject metaMessage)
        {
            return sendMetaMessage(metaMessage, _kafkaConfigHelper.ControllerTopicName);
        }

        private bool sendMetaMessage(JsonObject jsonMessage, string topicName)
        {
            try
            {
                JsonValue fileKey = jsonMessage["fileID"];
                string fileIDString = fileKey.ToString().Replace("\"", "", System.StringComparison.CurrentCulture);
                Guid fileGuid = Guid.Parse(fileIDString);
                _kafkaPublisher.SendMessage(jsonMessage.ToString(), topicName, fileGuid);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("MessagePublisher:sendMessage : Error while sending message to topic :{0}. Details :{1}", topicName, e);
            }

            return false;
        }
    }
}
