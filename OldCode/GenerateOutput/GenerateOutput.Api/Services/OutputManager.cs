using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Json;
using System.Linq;
using GenerateOutput.Models;
using Common.Lib.Kafka;
using System.Threading.Tasks;

namespace GenerateOutput.Api.Services
{
    public class OutputManager : IMessageHandler<IOutputManager>
    {
        readonly ILogger<IOutputManager> _logger;
        readonly IRPSDBProcessManager _RPSDBProcessManager;
        private const string PARTICIPANT_ALLOCATION_NAME = "participantAllocations";
        private const string METADATA_NAME = "fileProperties";

        public OutputManager(ILogger<IOutputManager> logger,IRPSDBProcessManager _rpsDBProcessManager)
        {
            _logger = logger;
            _RPSDBProcessManager = _rpsDBProcessManager;
        }

        public async Task<bool> HandleMessage(string key, string message)
        {
           return await handleMessage(key, message);
        }

        private async Task<bool> handleMessage(string key, string message)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(message) || String.IsNullOrWhiteSpace(key))
                {
                    _logger.LogCritical("Invalid input received by output manager: {0} : {1}", key, message);
                    return false;
                }
                var compensations = getCompensationList(message);
                _RPSDBProcessManager.SaveToDB(key, compensations);
                _logger.LogInformation("ProcessOutput for key: {0} complete. Number of Records: {1}, Generate Output: {2}", key, compensations?.Count, Compensation.GetCompensationsAsString(compensations));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Message is not valid!: {0}. Exception : {1}", message, ex);
            }

            return false;
        }

        private IList<Compensation> getCompensationList(string message)
        {
            JsonArray allocations = (JsonArray)JsonObject.Parse(message)[PARTICIPANT_ALLOCATION_NAME];
            JsonObject metaData = (JsonObject)JsonObject.Parse(message)[METADATA_NAME];

            IList<Compensation> compensations = new List<Compensation>();
            foreach (JsonValue allocation in allocations)
            {
                compensations.Add(new Compensation()
                {
                    // TODO: Set DivCode and RegCode to compensation object
                    AccountId = allocation["AccountId"].ToString(),
                    Code = allocation["Code"],
                    Value = allocation["Value"].ToString(),
                    ComplyDate = allocation["ComplyDate"],
                    PostId = metaData["dailyCyclePostID"],
                    SourceType = metaData["sourceType"].ToString(),
                    SourceCode = metaData["sourceCode"],
                    PlanCode = metaData["planCode"],
                    WireNumber = metaData["wireNumber"]
                });
            }
            return compensations;
        }
    }
}
