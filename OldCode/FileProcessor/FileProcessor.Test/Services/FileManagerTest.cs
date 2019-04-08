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

    public class FileManagerTest
    {

        Mock<ILogger<FileManager>> _loggerMock = new Mock<ILogger<FileManager>>();

        [Fact]
        public void FileManager_GetMetaData()
        {
            JsonObject jobject = TestHelper.getJsonObject();
            var objectUnderTest = new FileManager(_loggerMock.Object);
            //System.Diagnostics.Debugger.Launch();
            JsonObject metaData = objectUnderTest.GetMetaData(jobject);
            Assert.True("{\"bypass402gTest\": \"False\", \"complianceDate\": \"11/21/2018\", \"conduitEarningsActivityCode\": \"\", \"conduitEarningsAmount\": \"0\", \"conduitEarningsFlag\": \"False\", \"dailyCycleDate\": \"dailyCycleDateHere mm/dd/yyyy\", \"dailyCyclePostID\": \"dailyCyclePostIDHere ######\", \"fileColumns\": 38, \"fileDescription\": \"Sample Payroll File\", \"fileName\": \"PSPFJ0102\", \"filePath\": \"N:/NEWPARTS/DATA/PARTSPLANS/FJ0102/TransactionFiles/\", \"fileRows\": 1414, \"multipleWires\": \"True\", \"numericsFlag\": \"False\", \"planCode\": \"FJ0102\", \"planDailyFlagValue\": \"Daily\", \"preFundDepositAmount\": \"0\", \"preFundFlag\": \"False\", \"preFundInterest\": \"0\", \"preFundWireNumber\": \"0\", \"printDetailReport\": \"False\", \"printParticNoActivity\": \"False\", \"reRunFinancials\": \"False\", \"shareExchangeCCode\": \"AC53\", \"sourceCode\": \"wireNumberHere\", \"sourceType\": 106, \"splitBy\": \"Region\", \"splitFileFlag\": \"True\", \"splitFileFromMatchFlag\": \"False\", \"tapeNumber\": \"29000\", \"tblSourceForPSS\": \"\", \"threshold402g\": \"50\", \"updateMode\": \"UpdateModeHere\", \"wireNumber\": \"wireNumberHere\", \"workFileName\": \"D134195\"}" == metaData.ToString());
        }



        [Fact]
        public void FileManager_GetFileRules()
        {
            JsonObject jobject = TestHelper.getJsonObject();
            // System.Diagnostics.Debugger.Launch();
            var objectUnderTest = new FileManager(_loggerMock.Object);
            JsonArray fieldProps = objectUnderTest.GetFileRules(jobject);

            Assert.True(fieldProps.Count == 33);
        }

        [Fact]
        public void FileManager_GetCompensationFileRules()
        {
            JsonObject jobject = TestHelper.getJsonObject();
            //System.Diagnostics.Debugger.Launch();
            var objectUnderTest = new FileManager(_loggerMock.Object);
            var fieldProp = objectUnderTest.GetCompensationFileRules(jobject);
            Assert.True(fieldProp.Count == 2);

        }

        [Fact]
        public void FileManager_GetParticipants()
        {
            JsonObject jobject = TestHelper.getJsonObject();
            var objectUnderTest = new FileManager(_loggerMock.Object);
            // System.Diagnostics.Debugger.Launch();
            JsonArray participants = objectUnderTest.GetParticipants(jobject);
            Assert.True(participants.Count == 5);
        }

        [Theory]
        [InlineData("mtdcomp")]
        [InlineData("ptdother1")]
        public void FileManager_Participant_HasCompensationField(string compField)
        {
            JsonObject jobject = TestHelper.getParticipantObject_withCompensation();
            var objectUnderTest = new FileManager(_loggerMock.Object);
            //string compField = "mtdcomp";
            //  System.Diagnostics.Debugger.Launch();
            //TODO : GetCompensationParticipants later should be Get by data type param
            bool hasCompensation = objectUnderTest.ParticipantHasInvestment(jobject, compField);
            Assert.True(hasCompensation);
        }

        [Theory]
        [InlineData("accountid", "452061756")]
        [InlineData("mtdcomp", "5475.36")]
        [InlineData("ptdother1", "32852.14")]
        public void FileManager_Participant_RetrieveField(string compField, string expectedValue)
        {
            JsonObject jobject = TestHelper.getParticipantObject_withCompensation();
            var objectUnderTest = new FileManager(_loggerMock.Object);
            //string compField = "mtdcomp";
            //  System.Diagnostics.Debugger.Launch();
            //TODO : GetCompensationParticipants later should be Get by data type param
            string participantValue = objectUnderTest.GetParticipantFieldValue(jobject, compField).ToString();
            Assert.True(participantValue == expectedValue);
        }

        [Fact]
        public void FileManager_CreateCompensationInvestments()
        {
            // we need a rule, based on rule get one participant
            //then msg contains AccountId, Value from participanr, Code from Rule

            JsonObject jobject = TestHelper.getJsonObject();

            var fm = TestHelper.getFileInfo();
            JsonArray fs = TestHelper.getCompensationRules();
            JsonArray ps = TestHelper.GetParticipants();


            var objectUnderTest = new FileManager(_loggerMock.Object);

            //System.Diagnostics.Debugger.Launch();            

            List<CompensationInvestment> participants = objectUnderTest.CreateCompensationInvestments(fm, fs, ps, Guid.NewGuid());
            int expectedParticipantCount = 9;
            Assert.Equal(participants.Count, expectedParticipantCount);

        }

        [Fact]
        public void FileManager_CreateCompensationParticipant()
        {
            // we need a rule, based on rule get one participant
            //then msg contains AccountId, Value from participanr, Code from Rule

            JsonObject jobject = TestHelper.getJsonObject();

            var fm = TestHelper.getFileInfo();
            var fs = TestHelper.getCompensationRule();
            var p = TestHelper.getParticipantObject_withCompensation();


            var objectUnderTest = new FileManager(_loggerMock.Object);

            //System.Diagnostics.Debugger.Launch();            
            CompensationInvestment participant = objectUnderTest.CreateCompensationParticipant(fm, fs, p, Guid.NewGuid());

            Assert.NotNull(participant);
            Assert.True(participant.Code == "nu07");
            Assert.True(participant.AccountId == "452061756");
            Assert.True(participant.fileKey != null);

        }

        [Fact]
        public void FileManager_canCreateFileKey()
        {
            var objectUnderTest = new FileManager(_loggerMock.Object); ;
            Guid fileKey = objectUnderTest.FileKeyGenerator();
            Assert.NotEqual(fileKey, Guid.Empty);
        }


    }


}