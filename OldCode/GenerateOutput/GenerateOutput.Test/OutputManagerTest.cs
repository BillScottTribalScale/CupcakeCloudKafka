using Xunit;
using GenerateOutput.Api.Services;
using Moq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Extensions.Logging.Internal;
using GenerateOutput.Models;


namespace GenerateOutput.Test
{
    public class OutputManagerTest
    {
        Mock<ILogger<IOutputManager>> _logger = new Mock<ILogger<IOutputManager>>();
        Mock<IRPSDBProcessManager> _RPSDBProcessManagerMock = new Mock<IRPSDBProcessManager>();
        readonly String _validMessage = TestHelper.getJsonObject().ToString();
        readonly String _expectedGuidKey = Guid.NewGuid().ToString();

        [Fact]
        public void OutputManager_ProcessOutputMessage_LogErrorIfMessageIsInvalid()
        {
            // Arrange
            _logger.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _RPSDBProcessManagerMock.Setup(x=>x.SaveToDB(It.IsAny<string>(), It.IsAny<IList<Compensation>>()));
            String badMessage = "";

            OutputManager objectUnderTest = new OutputManager(_logger.Object, _RPSDBProcessManagerMock.Object);

            // Action
            var result = objectUnderTest.HandleMessage(_expectedGuidKey, badMessage).Result;

            // Assert
            _logger.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
            _RPSDBProcessManagerMock.Verify(x=>x.SaveToDB(It.IsAny<string>(), It.IsAny<IList<Compensation>>()), Times.Never());
           
         }

         [Fact]
        public void OutputManager_ProcessOutputMessage_LogErrorIfKeyIsInvalid()
        {
            // Arrange
            String badKey = "";
            _logger.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _RPSDBProcessManagerMock.Setup(x=>x.SaveToDB(It.IsAny<string>(), It.IsAny<IList<Compensation>>()));
            
            OutputManager objectUnderTest = new OutputManager(_logger.Object, _RPSDBProcessManagerMock.Object);

            // Action
            var result = objectUnderTest.HandleMessage(badKey, _validMessage).Result;

            // Assert
            _logger.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
             _RPSDBProcessManagerMock.Verify(x=>x.SaveToDB(It.IsAny<string>(), It.IsAny<IList<Compensation>>()), Times.Never());
        }

        [Fact]
        public void OutputManager_ProcessOutputMessage()
        {
            _logger.Setup(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _RPSDBProcessManagerMock.Setup(x=>x.SaveToDB(It.IsAny<string>(), It.IsAny<IList<Compensation>>()));
    
            OutputManager objectUnderTest = new OutputManager(_logger.Object, _RPSDBProcessManagerMock.Object);

            var result =  objectUnderTest.HandleMessage("key1",_validMessage).Result;
            
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
            _RPSDBProcessManagerMock.Verify(x=>x.SaveToDB(It.IsAny<string>(), It.IsAny<IList<Compensation>>()), Times.Once());
        }

        [Fact]
        public void OutputManager_ProcessOutputMessage_Verify_Compensations()
        {
            var compensationList = new List<Compensation>();
            _logger.Setup(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _RPSDBProcessManagerMock.Setup(x=>x.SaveToDB(It.IsAny<string>(), It.IsAny<IList<Compensation>>()))
            .Callback<string, IList<Compensation>>((c1, c2) => { compensationList = c2.ToList(); });;
    
            OutputManager objectUnderTest = new OutputManager(_logger.Object, _RPSDBProcessManagerMock.Object);
            var result =  objectUnderTest.HandleMessage("key1",_validMessage).Result;
            //Assert
            Assert.True(compensationList.Count == 5);
            Assert.True(compensationList[0].AccountId == "\"T123456\"");
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
            _RPSDBProcessManagerMock.Verify(x=>x.SaveToDB(It.IsAny<string>(), It.IsAny<IList<Compensation>>()), Times.Once());
        }

        [Fact]
        public void OutputManager_ProcessOutputMessage_When_Exception()
        {
            _logger.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _RPSDBProcessManagerMock.Setup(x=>x.SaveToDB(It.IsAny<string>(), It.IsAny<IList<Compensation>>()));
    
            OutputManager objectUnderTest = new OutputManager(_logger.Object, _RPSDBProcessManagerMock.Object);
            var invalidMessage = _validMessage.Replace("participantAllocations","participantBadAllocations");
            var result = objectUnderTest.HandleMessage("key1",invalidMessage).Result;
            
            _logger.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once);
            _RPSDBProcessManagerMock.Verify(x=>x.SaveToDB(It.IsAny<string>(), It.IsAny<IList<Compensation>>()), Times.Never());
        }        
    }
}
