using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Lib.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GenerateOutput.Api.Services
{
    /// <summary>
    /// PayDayConnector class is used to run the calling method as back ground process.  
    /// </summary>
    public class PayDayConnector : BackgroundService
    {
        private readonly ILogger<PayDayConnector> _logger;
        private IKafkaConsumer _consumer;
        private readonly IKafkaConfiguration _configuration;
        private IMessageHandler _outputHandler;
        public PayDayConnector(ILogger<PayDayConnector> logger, IKafkaConsumer consumer, IKafkaConfiguration configuration, IMessageHandler<IOutputManager> outputHandler)
        {
            _logger = logger;
            _consumer = consumer;
            _configuration = configuration;
            _outputHandler = outputHandler;
        }

        /// <summary>
        /// This method is implemented to support BackgroundService 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.ProcessMessages(stoppingToken, _configuration.PlanTopicName, _outputHandler);
            await Task.CompletedTask;
        }
        public override void Dispose()
        {
            _consumer = null;
            base.Dispose();
        }

    }
}
