using System.Json;
using FileProcessor.Api.Models;
using System;
using System.Collections.Generic;

namespace FileProcessor.Api.Services
{

    public interface IMessageManager
    {
        ProcessSummary CreateProcessSummary(DateTime startTime, DateTime endTime, JsonObject fileMeta, JsonArray fileRules, JsonArray participants, IList<CompensationInvestment> investments, Guid fileKey);
        JsonObject CreateMetaMessage(JsonObject metaFile, string fileID, int participantCount);

    }

}