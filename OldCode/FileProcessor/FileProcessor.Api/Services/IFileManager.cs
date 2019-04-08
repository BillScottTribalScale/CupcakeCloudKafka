using System.Json;
using System.Collections.Generic;
using FileProcessor.Api.Models;
using System;

namespace FileProcessor.Api.Services
{

    public interface IFileManager
    {
        string ReadFile(string filename);
        JsonObject GetMetaData(JsonObject context);
        JsonArray GetFileRules(JsonObject context);

        JsonArray GetParticipants(JsonObject context);

        JsonArray GetCompensationFileRules(JsonObject context);

        bool ParticipantHasInvestment(JsonObject participant, string compField);
        List<CompensationInvestment> CreateCompensationInvestments(JsonObject fileMeta, JsonArray fileRules, JsonArray participants, Guid fileKey);
        CompensationInvestment CreateCompensationParticipant(JsonObject fileMeta, JsonObject fileRule, JsonObject participant, Guid fileKey);
        Guid FileKeyGenerator();
    }

}