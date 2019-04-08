using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Common.Lib.Kafka
{
    /// <summary>
    /// KafkaConsumer class is responsible for consuming the Kafka messages
    /// </summary>
    public class KafkaConsumer : IKafkaConsumer
    {
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly IConsumer<string, string> _consumer;
        public KafkaConsumer(ILogger<KafkaConsumer> logger, IConsumer<string, string> consumer)
        {
            _logger = logger;
            _consumer = consumer;
        }

        /// <summary>
        /// This method is used to consume the Kafka message to process the same to listener
        /// </summary>
        /// <param name="stoppingToken">cancellation token triggered when background process shuts down</param>
        /// <param name="topicName">kafka Topic to consume</param>
        public void ProcessMessages(CancellationToken stoppingToken, string topicName, IMessageHandler messageHandler)
        {
            processMessagesAsync(stoppingToken, topicName, messageHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <param name="handlers"></param>
        public void ProcessMessages(CancellationToken stoppingToken, IDictionary<string,IMessageHandler> handlers)
        {          
            processMessagesAsync(stoppingToken, handlers).ConfigureAwait(false);
        }        
        private async Task processMessagesAsync(CancellationToken stoppingToken, IDictionary<string, IMessageHandler> handlers)
        {
            try
            {
                _consumer.Subscribe(handlers.Keys);
                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(500));
                    if (consumeResult != null)
                    {                        
                        if(handlers.TryGetValue(consumeResult.Topic,out IMessageHandler handler))
                        {
                            await handler.HandleMessage(consumeResult.Key, consumeResult.Value);
                        }
                    }
                }
            }
            catch (ConsumeException e)
            {
                _logger.LogError("KafkaConsumer:ProcessMessages - Error while receiving Kafka message. Error Code :{0}, Message : {1}", e.Error.Code, e.Error.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("KafkaConsumer:ProcessMessages - Error while receiving Kafka message. Error Details : {0}", ex);
            }
        }
        private async Task processMessagesAsync(CancellationToken stoppingToken, string topicName, IMessageHandler messageHandler)
        {
            try
            {
                _consumer.Subscribe(topicName);
                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(100));
                    if (consumeResult != null)
                    {
                        await messageHandler.HandleMessage(consumeResult.Key, consumeResult.Value);
                    }
                }
            }
            catch (ConsumeException e)
            {
                _logger.LogError("KafkaConsumer:ProcessMessages - Error while receiving Kafka message. Error Code :{0}, Message : {1}", e.Error.Code, e.Error.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("KafkaConsumer:ProcessMessages - Error while receiving Kafka message. Error Details : {0}", ex);
            }
        }              
    }
}
