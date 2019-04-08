using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Lib.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aggregator.Api.Services
{
    /// <summary>
    /// PayDayConnector class is used to run the calling method as back ground process.  
    /// </summary>
    public class PayDayConnector : BackgroundService
    {
        private readonly ILogger<PayDayConnector> _logger;
        private IKafkaConsumer _consumer;
        private readonly IKafkaConfiguration _configuration;
        private IMessageHandler _outputAllocationHandler;
        private IMessageHandler _outputMetaHandler;
        Dictionary<string,IMessageHandler> _handlers;
        public PayDayConnector(ILogger<PayDayConnector> logger, IKafkaConsumer consumer, IKafkaConfiguration configuration,
            IMessageHandler<IAllocationManager> outputAllocationHandler, IMessageHandler<IMetaManager> outputMetaHandler)
        {
            _logger = logger;
            _consumer = consumer;
            _configuration = configuration;
            _outputAllocationHandler = outputAllocationHandler;
            _outputMetaHandler = outputMetaHandler;
            setHandlers();
        }

        /// <summary>
        /// This method is implemented to support BackgroundService 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {            
            _consumer.ProcessMessages(stoppingToken, _handlers);
            await Task.CompletedTask;
        }
        public override void Dispose()
        {
            _consumer = null;
            base.Dispose();
        } 
        private void setHandlers()
        {
            _handlers = new Dictionary<string, IMessageHandler>();
            _handlers.Add(_configuration.ControllerTopicName, _outputMetaHandler);
            _handlers.Add(_configuration.ParticipantAllocationsTopicName, _outputAllocationHandler);
        }      
    }
}
