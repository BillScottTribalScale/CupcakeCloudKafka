using Xunit;
using Aggregator.Api.Services;
using Moq;
using Microsoft.Extensions.Logging;

using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Json;
using System;
using Common.Lib.Kafka;
using Microsoft.Extensions.Logging.Internal;

namespace Aggregator.Test
{
    public class AggregationManagerTest
    {
        /// X Subscribe to Meta Topic
        /// X Subscribe to allocation topic
        /// X when meta message event, store the fileID and # of records
        /// -----------  retrieve # records value from JSON object
        /// X  when allocation message event, store fileID and message contents
        /// X When either event, execute comparison algorithm (check for message count == # of records)
        /// X     if comparison = false, do nothing.
        /// X   if comparison = true, log message that successful
        ///     also, add message to results KAFKA topic
        ///          Steps)
        ////             1)Create message for plan results
        ///              2) Publish plan results to the topic
        /// TODO: tbd defined what next action is!!!

        Mock<ILogger<IAggregationManager>> _logger = new Mock<ILogger<IAggregationManager>>();
        List<MetaMessage> _metaRepo = new List<MetaMessage>();
        List<AllocationMessage> _allocationRepo = new List<AllocationMessage>();

        Mock<IKafkaPublisher> _publisher = new Mock<IKafkaPublisher>();
        string _fakeMessage = "any string here";

        const string participantAmountKeyword = "participantAmountCount";

        [Fact]
        public void AggregationTest_AddToAllocationRepo()
        {
            // Assemble
            Guid fileID = Guid.NewGuid();
            int count = 1;
            // Action            
            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);
            objectUnderTest.AddToAllocationRepo(fileID.ToString(), count.ToString());
            // Assert  
            Assert.True(objectUnderTest._allocationRepo.Count == 1);
        }

        [Fact]
        public void AggregationTest_AddToAllocationRepo_When_Exception()
        {
            // Assemble            
            int count = 1;
            _logger.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

            // Action            
            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);
            Action act = () => objectUnderTest.AddToAllocationRepo(null, count.ToString());
            // Assert  
            Assert.Throws<Exception>(act);
            _logger.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());

        }
        [Fact]
        public void AggregationTest_AddToMetaRepo()
        {
            // Assemble
            int count = 1;
            // Action            
            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);
            objectUnderTest.AddToMetaRepo("testkey", count, _fakeMessage);
            // Assert  
            Assert.True(objectUnderTest._metaRepo.Count == 1);
        }

        [Fact]
        public void AggregationTest_AddToMetaRepo_When_Exception()
        {
            // Assemble            
            int count = 1;
            _logger.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

            // Action            
            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);
            Action act = () => objectUnderTest.AddToMetaRepo(string.Empty, count, _fakeMessage);
            // Assert  
            Assert.Throws<Exception>(act);
            _logger.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());

        }

        [Fact]
        public void AggregationTest_GetMetaCount_When_MessageIsEmpty_Exception()
        {
            //assemble
            int expectedValue = 0;
            _logger.Setup(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);

            //ACTION
            var result = objectUnderTest.GetMetaCount("");

            //assert
            Assert.True(result == expectedValue);
            _logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
        }

        [Fact]
        public void AggregationTest_GetMetaCount_IfMetaMessageIsNull_Exception()
        {
            //Assemble
            int expectedValue = 0;
            _logger.Setup(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);

            //Action
             var result = objectUnderTest.GetMetaCount(null);

            //Assert
            Assert.True(result == expectedValue);
            _logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
        }

        [Fact]
        public void AggregationTest_GetMetaCount_IfNoParticipantAmountKeyword_Exception()
        {
            //Assemble
            int expectedValue = 0;
            JsonObject fakeMessage = TestHelper.getFileInfo();
            fakeMessage.Add("fileID", Guid.NewGuid().ToString());
            string jsonMessage = fakeMessage.ToString();
            _logger.Setup(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
    
            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);

            //Action
             var result = objectUnderTest.GetMetaCount(jsonMessage);

            //Assert
            Assert.True(result == expectedValue);
            _logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
        }

        [Fact]
        public void AggregationTest_GetMetaCount_ReturnExpectedValue()
        {
            //assemble
            JsonObject fakeMessage = TestHelper.getFileInfo();
            fakeMessage.Add("fileID", Guid.NewGuid().ToString());
            fakeMessage.Add(participantAmountKeyword, 100);
            string jsonMessage = fakeMessage.ToString();
            int expectedValue = 100;

            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);

            //action
            int actualValue = objectUnderTest.GetMetaCount(jsonMessage);

            //assert
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void AggregationTest_GetMetaCountByKey_With_ValidKey()
        {
            //assemble
            int expectedValue = 1;
            _logger.Setup(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);
            //action
            objectUnderTest.AddToMetaRepo("testkey", 1, _fakeMessage);

            int actualValue = objectUnderTest.GetMetaCountByKey("testkey");
            //assert
            Assert.Equal(expectedValue, actualValue);
            _logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Never());
        }

        [Fact]
        public void AggregationTest_GetMetaCountByKey_With_InvalidKey()
        {
            //assemble
            int expectedValue = 1;
            _logger.Setup(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);
            //action
            objectUnderTest.AddToMetaRepo("testkey", 1, _fakeMessage);

            int actualValue = objectUnderTest.GetMetaCountByKey("blahblah");
            //assert
            Assert.NotEqual(expectedValue, actualValue);
            _logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Never());
        }

        [Fact]
        public void AggregationTest_GetMetaCountByKey_With_NullOrEmptyKey()
        {
            //assemble
            int expectedValue = 1;
            _logger.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);
            //action
            int actualValue = objectUnderTest.GetMetaCountByKey(string.Empty);
            //assert
            Assert.NotEqual(expectedValue, actualValue);
            _logger.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
        }

        [Fact]
        public void AggregationManager_EnrichAndSendMessage()
        {
            //Arrange
            string key = Guid.Parse("247d7eb3-2fbe-4b3c-84ef-f3580f5c6331").ToString();
            string actualMessageToPublish = getActualMessage();
            string expectedMessageToPublish = string.Empty;
            _publisher.Setup(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Callback<string, string, Guid>((c1, c2, c3) => { expectedMessageToPublish = c1; });

            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);
            objectUnderTest.AddToMetaRepo(key, 1, getMetaMessage(key));
            objectUnderTest.AddToAllocationRepo(key, @"{""someJasonName"":""somevalue1""}");
            objectUnderTest.AddToAllocationRepo(key, @"{""someJasonName"":""somevalue2""}");
            objectUnderTest.AddToAllocationRepo(key, @"{""someJasonName"":""somevalue3""}");
            //Action
            var result = objectUnderTest.EnrichAndSendMessage(key, Guid.Parse(key), 3, "topic");

            //Assert
            Assert.True(result);
            Assert.True(actualMessageToPublish == expectedMessageToPublish);
            _publisher.Verify(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Once());
        }

        [Fact]
        public void AggregationManager_EnrichAndSendMessage_WhenNoAllocationMessage()
        {
            //Arrange
            string key = Guid.Parse("247d7eb3-2fbe-4b3c-84ef-f3580f5c6331").ToString();
            string actualMessageToPublish = getActualMessage();
            _publisher.Setup(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()));
            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);
            objectUnderTest.AddToMetaRepo(key, 1, getMetaMessage(key));

            //Action
            var result = objectUnderTest.EnrichAndSendMessage(key, Guid.Parse(key), 3, "topic");

            //Assert
            Assert.True(result);
            _publisher.Verify(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never());
        }

        [Fact]
        public void AggregationManager_EnrichAndSendMessage_When_FileAllocationCountIsZero()
        {
            //Arrange
            int fileAllocationCount = 0;
            string key = Guid.Parse("247d7eb3-2fbe-4b3c-84ef-f3580f5c6331").ToString();
            _publisher.Setup(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()));
            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);
            objectUnderTest.AddToAllocationRepo(key, @"{""someJasonName"":""somevalue1""}");
            //Action
            var result = objectUnderTest.EnrichAndSendMessage(key, Guid.Parse(key), fileAllocationCount, "topic");

            //Assert
            Assert.True(result);
            _publisher.Verify(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never());
        }

        [Fact]
        public void AggregationManager_EnrichAndSendMessage_When_Exception_While_Enriching()
        {
            //Arrange
            int fileAllocationCount = 1;
            string key = Guid.Parse("247d7eb3-2fbe-4b3c-84ef-f3580f5c6331").ToString();
            _logger.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _publisher.Setup(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()));
            AggregationManager objectUnderTest = new AggregationManager(_logger.Object, _publisher.Object, _allocationRepo, _metaRepo);
            objectUnderTest.AddToAllocationRepo(key, "{'someJasonName':'somevalue1'}");
            //Action
            var result = objectUnderTest.EnrichAndSendMessage(key, Guid.Parse(key), fileAllocationCount, "topic");

            //Assert
            Assert.False(result);
            _publisher.Verify(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Never());
            _logger.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());

        }

        private static string getActualMessage()
        {
            return "{\"fileID\": \"247d7eb3-2fbe-4b3c-84ef-f3580f5c6331\", \"fileProperties\": {\"bypass402gTest\": \"False\", \"complianceDate\": \"11/21/2018\", \"conduitEarningsActivityCode\": \"\", \"conduitEarningsAmount\": \"0\", \"conduitEarningsFlag\": \"False\", \"dailyCycleDate\": \"dailyCycleDateHere mm/dd/yyyy\", \"dailyCyclePostID\": \"dailyCyclePostIDHere ######\", \"fileColumns\": 38, \"fileDescription\": \"Sample Payroll File\", \"fileID\": \"247d7eb3-2fbe-4b3c-84ef-f3580f5c6331\", \"fileName\": \"PSPFJ0102\", \"filePath\": \"N:/NEWPARTS/DATA/PARTSPLANS/FJ0102/TransactionFiles/\", \"fileRows\": 1414, \"multipleWires\": \"True\", \"numericsFlag\": \"False\", \"participantAmountCount\": 100, \"planCode\": \"FJ0102\", \"planDailyFlagValue\": \"Daily\", \"preFundDepositAmount\": \"0\", \"preFundFlag\": \"False\", \"preFundInterest\": \"0\", \"preFundWireNumber\": \"0\", \"printDetailReport\": \"False\", \"printParticNoActivity\": \"False\", \"reRunFinancials\": \"False\", \"shareExchangeCCode\": \"AC53\", \"sourceCode\": \"wireNumberHere\", \"sourceType\": 106, \"splitBy\": \"Region\", \"splitFileFlag\": \"True\", \"splitFileFromMatchFlag\": \"False\", \"tapeNumber\": \"29000\", \"tblSourceForPSS\": \"\", \"threshold402g\": \"50\", \"updateMode\": \"UpdateModeHere\", \"wireNumber\": \"wireNumberHere\", \"workFileName\": \"D134195\"}, \"participantAllocations\": [{\"someJasonName\": \"somevalue1\"}, {\"someJasonName\": \"somevalue2\"}, {\"someJasonName\": \"somevalue3\"}]}";
        }

        private static string getMetaMessage(string key)
        {
            var metaMessage = TestHelper.getFileInfo();
            metaMessage.Add("fileID", key);
            metaMessage.Add("participantAmountCount", 100);
            string jsonMessage = metaMessage.ToString();
            return jsonMessage;
        }
    }
}
