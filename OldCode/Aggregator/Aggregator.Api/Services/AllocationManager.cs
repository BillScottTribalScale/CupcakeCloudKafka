using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Threading.Tasks;
using Common.Lib.Kafka;
using Microsoft.Extensions.Logging;

namespace Aggregator.Api.Services
{
    public class AllocationManager : IMessageHandler<IAllocationManager>
    {
        readonly ILogger<IAllocationManager> _logger;
        readonly IKafkaConfiguration _configuration;
        readonly IAggregationManager _aggregationManager;

        public AllocationManager(ILogger<IAllocationManager> logger, IKafkaConfiguration configuration, IAggregationManager aggregationManager)
        {
            _logger = logger;
            _configuration = configuration;
            _aggregationManager = aggregationManager;
        }

        public async Task<bool> HandleMessage(string key, string message)
        {
            return await handleMessage(key, message);
        }

        private async Task<bool> handleMessage(string key, string message)
        {
            try
            {
                Guid fileGuid = Guid.Parse(key);

                _aggregationManager.AddToAllocationRepo(key, message);
                var metaCount = _aggregationManager.GetMetaCountByKey(key);

                return _aggregationManager.EnrichAndSendMessage(key, fileGuid, metaCount, _configuration.PlanTopicName);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("AllocationManager : HandleMessage Unable to enrich and send message", ex);
                return false;
            }
        }
    }
}