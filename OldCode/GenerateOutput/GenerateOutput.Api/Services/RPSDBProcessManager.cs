using System;
using System.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using GenerateOutput.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Data;

namespace GenerateOutput.Api.Services
{
    public class RPSDBProcessManager : IRPSDBProcessManager
    {
        readonly ILogger<IRPSDBProcessManager> _logger;

        public RPSDBProcessManager(ILogger<IRPSDBProcessManager> logger)
        {
            _logger = logger;
        }

        public bool SaveToDB(string fileKey, IList<Compensation> compensations)
        {
            //Add to Kibana log
            if(!compensations.Any())
            {
                _logger.LogCritical("RPSDBProcessManager : SaveToDB. No compensation data to save for file {0}",fileKey);
                return false;
            }
            
            _logger.LogInformation("RPSDBProcessManager SaveToDB for key: {0} complete. Number of Records: {1}, Generate Output: {2}"
            , fileKey,compensations.Count,Compensation.GetCompensationsAsString(compensations,false));
            //TODO : 
            //  1. Depending on sql call as well as async/sync operation, reconsider return value.
            //     right now it is just a pass through.
            // 2. Call SP based on table param(or any other) that sp expect.
            return true;
         }         
    }
}