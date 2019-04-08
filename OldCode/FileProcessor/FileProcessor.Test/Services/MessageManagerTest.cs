using System.Json;
using FileProcessor.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Collections.Generic;
using FileProcessor.Api.Models;
using System;

namespace FileProcessor.Test
{
    public class MessageManagerTest
    {
        Mock<ILogger<FileManager>> _fileLoggerMock = new Mock<ILogger<FileManager>>();
        Mock<ILogger<MessageManager>> _loggerMock = new Mock<ILogger<MessageManager>>();
        Mock<IFileManager> _reader = new Mock<IFileManager>();

        [Fact]
        public void MessageManager_CanCreateSummary()
        {

            JsonObject jobject = TestHelper.getJsonObject();

            var fm = TestHelper.getFileInfo();
            JsonArray fs = TestHelper.getCompensationRules();
            JsonArray ps = TestHelper.GetParticipants();
            List<CompensationInvestment> invests = TestHelper.GenerateCompensationInvestments();
            var objectUnderTest = new MessageManager(_loggerMock.Object);
            var sum = objectUnderTest.CreateProcessSummary(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow, fm, fs, ps, invests, Guid.NewGuid());

            Assert.NotNull(sum);

        }

        [Fact]
        public void MessageManager_Metadata_ReturnsEnrichedMeta()
        {
            JsonObject TestInput = TestHelper.getJsonObject();
            var fileManager = new FileManager(_fileLoggerMock.Object);
            var objectUnderTest = new MessageManager(_loggerMock.Object);

            JsonObject metaData = fileManager.GetMetaData(TestInput);

            string ParticipantCountFieldName = "participantAmountCount";
            string FileIDFieldName = "fileID";

            string FileIDExpectedValue = Guid.NewGuid().ToString();
            int ParticipantCountFieldNameExpectedValue = 100;

            JsonObject enrichedMeta = objectUnderTest.CreateMetaMessage(metaData, FileIDExpectedValue, ParticipantCountFieldNameExpectedValue);

            Assert.True(enrichedMeta.Keys.Contains(ParticipantCountFieldName));
            Assert.True(enrichedMeta.Keys.Contains(FileIDFieldName));

            Assert.Equal(enrichedMeta[FileIDFieldName], FileIDExpectedValue);
            Assert.Equal(enrichedMeta[ParticipantCountFieldName].ToString(), ParticipantCountFieldNameExpectedValue.ToString());
        }
    }
}
