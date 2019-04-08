using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;

namespace Common.Lib.Kafka
{
    public interface IKafkaConsumer
    {           
        /// <summary>
        /// This method is used to consume the Kafka message to process the same to listener
        /// </summary>
        /// <param name="stoppingToken">cancellation token triggered when background process shuts down</param>
        /// <param name="topicName">kafka Topic to consume</param>
        /// <param name="messageHandler"></param>
        void ProcessMessages(CancellationToken stoppingToken, string topicName, IMessageHandler messageHandler);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <param name="handlers"></param>
        void ProcessMessages(CancellationToken stoppingToken, IDictionary<string,IMessageHandler> handlers);
        
    }
}
