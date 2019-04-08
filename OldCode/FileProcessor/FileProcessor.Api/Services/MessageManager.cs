using System;
using System.Collections.Generic;
using System.Json;
using Microsoft.Extensions.Logging;

using System.Diagnostics;
using FileProcessor.Api.Models;

namespace FileProcessor.Api.Services
{
    public class MessageManager : IMessageManager
    {
        private ILogger<MessageManager> _logger;

        private const string PARTICIPANT_AMOUNT_COUNT_FIELDNAME = "participantAmountCount";
        private const string FILE_ID_FIELD_NAME = "fileID";

        public MessageManager(ILogger<MessageManager> logger)
        {
            _logger = logger;
        }

        public ProcessSummary CreateProcessSummary(DateTime startTime, DateTime endTime, JsonObject fileMeta, JsonArray fileRules, JsonArray participants, IList<CompensationInvestment> investments, Guid fileKey)
        {
            ProcessSummary sum = new ProcessSummary();
            sum.StartTime = startTime;
            sum.EndTime = endTime;
            sum.FileName = fileMeta["fileName"];
            sum.FileKey = fileKey.ToString();
            sum.RulesDecoded = fileRules != null ? fileRules.Count : 0;
            sum.Participants = participants != null ? participants.Count : 0;
            sum.InvestmentsCreated = investments != null ? investments.Count : 0;
            return sum;
        }

        public JsonObject CreateMetaMessage(JsonObject metaFile, string fileID, int participantCount)
        {
            metaFile.Add(PARTICIPANT_AMOUNT_COUNT_FIELDNAME, participantCount);
            metaFile.Add(FILE_ID_FIELD_NAME, fileID);
            return metaFile;
        }
    }
}
