using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Lib.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaListener.Api.Services
{
    /// <summary>
    /// PayDayConnector class is used to run the calling method as back ground process.  
    /// </summary>
    public class PayDayConnector : BackgroundService
    {

        private readonly ILogger<PayDayConnector> _logger;
        private IKafkaConsumer _consumer;
        private readonly string _topicName = "PAYDAY_PARTICIPANT_V1_DEV";

        IMessageHandler _calcEngineHandler;

        //TODO:: messagehandler type should represent the topic
        public PayDayConnector(ILogger<PayDayConnector> logger, IKafkaConsumer consumer, IMessageHandler<ICalculationEngineService> calcEngineHandler)
        {
            _logger = logger;
            _consumer = consumer;
            _calcEngineHandler = calcEngineHandler;
        }

        /// <summary>
        /// This method is implemented to support BackgroundService 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.ProcessMessages(stoppingToken, _topicName, _calcEngineHandler);
            await Task.CompletedTask;
        }
        public override void Dispose()
        {
            _consumer = null;
            base.Dispose();
        }

    }
}
