using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Common.Lib.Kafka
{
    /// <summary>
    /// KafkaPublisher : Responsible for sending message to Kafka Broker based on kafka topic
    /// </summary>
    public class KafkaPublisher : IKafkaPublisher
    {
        private readonly ILogger<KafkaPublisher> _logger;
        private readonly IProducer<string, string> _producer;

        /// <summary>
        /// Constructor with DI
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="producer"></param>
        public KafkaPublisher(ILogger<KafkaPublisher> logger, IProducer<string, string> producer)
        {
            _logger = logger;
            _producer = producer;
        }

        /// <summary>
        ///  This method is to send message to Kafka
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="topicName">Topic Name</param>
        /// <param name="fileKey"></param>
        /// <returns></returns>
        public void SendMessage(string message, string topicName, Guid fileKey)
        {
            sendMessage(message, topicName, fileKey);
        }

        private async Task sendMessage(string message, string topicName, Guid fileKey)
        {
            try
            {
                await _producer.ProduceAsync(topicName,
                                        new Message<string, string> { Key = fileKey == Guid.Empty ? null : fileKey.ToString(), Value = message }
                                        );
            }
            catch (Exception e)
            {
                //TODO: WHAT DO WE DO WHEN AN ERROR POSTING TO KAFKA?
                _logger.LogError("KafkaPublisher:sendMessage Error while sending message to {0} Topic. Details : {1}",topicName, e);
            }
        }
    }
}
