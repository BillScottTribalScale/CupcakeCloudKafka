using System;
using System.Collections.Generic;
using System.Json;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using FileProcessor.Api.Models;

namespace FileProcessor.Api.Services
{
    public class FileProcessorService : IFileProcessorService
    {
        private readonly ILogger<FileProcessorService> _logger;
        private readonly IFileManager _fileManager;
        private readonly IMessageManager _messageManager;
        private readonly IMessagePublisher _publisher;

        public FileProcessorService(
            ILogger<FileProcessorService> logger,
            IFileManager fileManager, IMessageManager messageManager, IMessagePublisher publisher)
        {
            _logger = logger;
            _fileManager = fileManager;
            _messageManager = messageManager;
            _publisher = publisher;
        }
        public ProcessSummary StartProcess(string fileContent)
        {
            DateTime startTime = DateTime.UtcNow;
            var objJsonContent = (JsonObject)JsonObject.Parse(fileContent);
            var fileMetaData = _fileManager.GetMetaData(objJsonContent);
            var fileRules = _fileManager.GetCompensationFileRules(objJsonContent);
            var fileParticipants = _fileManager.GetParticipants(objJsonContent);
            var fileKey = _fileManager.FileKeyGenerator();
            var investments = _fileManager.CreateCompensationInvestments(fileMetaData, fileRules, fileParticipants, fileKey);
            _publisher.SendParticipantMessages(investments);
            ProcessSummary sum = _messageManager.CreateProcessSummary(startTime, DateTime.UtcNow, fileMetaData, fileRules, fileParticipants, investments, fileKey);
            int investmentCount = (investments != null ? investments.Count : 0);
            JsonObject enrichedMeta = _messageManager.CreateMetaMessage(fileMetaData, fileKey.ToString(), investmentCount);
            _publisher.SendMetaMessages(enrichedMeta);
            _logger.LogInformation("File completed processing at : {0}. /nFile Summary : # of Rules - {1} # of Participants {2} # of investments - {3} ",
            DateTime.UtcNow, fileRules?.Count, fileParticipants?.Count, investments?.Count);
            return sum;
        }
    }
}
