using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Json;
using System.Linq;
using Common.Lib.Kafka;
using System.Threading.Tasks;

namespace Aggregator.Api.Services
{
    public class AggregationManager : IAggregationManager
    {
        const string _participantAmountKeyword = "participantAmountCount";
        public IList<AllocationMessage> _allocationRepo { get; }
        public IList<MetaMessage> _metaRepo { get; }
        readonly ILogger<IAggregationManager> _logger;
        readonly IKafkaPublisher _publisher;

        public AggregationManager(ILogger<IAggregationManager> allocationLogger, IKafkaPublisher publisher, IEnumerable<AllocationMessage> allocationRepo, IEnumerable<MetaMessage> metaRepo)
        {
            _logger = allocationLogger;
            _publisher = publisher;
            _allocationRepo = allocationRepo.ToList();
            _metaRepo = metaRepo.ToList();
        }

        public void AddToAllocationRepo(string key, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    _logger.LogCritical("AggregationManager : AddToAllocationRepo file key cannot be null or empty");
                    throw new Exception("AggregationManager : AddToAllocationRepo file key cannot be null or empty");
                }
                _allocationRepo.Add(new AllocationMessage { Key = key, Value = message });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void AddToMetaRepo(string key, int allocationCount, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    _logger.LogCritical("AggregationManager : AddToMetaRepo file key cannot be null or empty");
                    throw new Exception("AggregationManager : AddToMetaRepo file key cannot be null or empty");
                }
                _metaRepo.Add(new MetaMessage() { Key = key, Count = allocationCount, MetaObject = message });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetMetaCount(string metaMessage)
        {
            try
            {
                JsonValue metaObject = JsonObject.Parse(metaMessage);
                return int.Parse(metaObject[_participantAmountKeyword].ToString());
            }
            catch (Exception ex)
            {
               _logger.LogError("AggregationManager:GetMetaCount - Error parsing for ParticipantAmountCount.Details : {0}", ex);
               return 0;
            }
        }
        public int GetMetaCountByKey(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    _logger.LogCritical("AggregationManager : GetMetaCountByKey File key can not be null or empty");
                    return 0;
                }
                var meta = _metaRepo.Where(x => x.Key == key).FirstOrDefault();
                return meta != null ? meta.Count : 0;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("AggregationManager : GetMetaCountByKey Unable to get meta count", ex);
                return 0;
            }

        }
        public bool EnrichAndSendMessage(string key, Guid fileGuid, int fileAllocationCount, string topicName)
        {
            try
            {
                int processedAllocationCount = _allocationRepo.Count(x => x.Key == key);
                //When allocation for a file is complete send message                
                if ((fileAllocationCount > 0 && processedAllocationCount >= fileAllocationCount))
                {
                    _publisher.SendMessage(getEnrichedMessage(key).ToString(), topicName, fileGuid); 
                    return true;   
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("AggregationManager : EnrichAndSendMessage error while sending message", ex);
            }
            return false;
        }
        private JsonObject getEnrichedMessage(string fileKey)
        {
            JsonArray allocation = new JsonArray(_allocationRepo.Where(x => x.Key == fileKey).Select(y => JsonValue.Parse(y.Value)));
            JsonObject metaObject = JsonObject.Parse(_metaRepo.Where(x => x.Key == fileKey).FirstOrDefault().MetaObject) as JsonObject;

            JsonObject enrichedMessage = new JsonObject();
            enrichedMessage.Add("fileID", fileKey);
            enrichedMessage.Add("fileProperties", metaObject);
            enrichedMessage.Add("participantAllocations", allocation);

            return enrichedMessage;
        }
    }
}
