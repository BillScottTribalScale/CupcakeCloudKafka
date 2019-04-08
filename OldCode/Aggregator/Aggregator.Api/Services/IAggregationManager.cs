using System;
using System.Json;

namespace Aggregator.Api.Services
{
    public interface IAggregationManager
    {
         void AddToAllocationRepo(string key, string message);
         void AddToMetaRepo(string key, int allocationCount, string message);
         int GetMetaCount(string metaMessage);
         int GetMetaCountByKey(string key);
         bool EnrichAndSendMessage(string key,Guid fileGuid, int fileAllocationCount, string topicName );
    }
}