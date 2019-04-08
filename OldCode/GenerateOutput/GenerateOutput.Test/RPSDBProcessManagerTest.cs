using System;
using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using Xunit;
using GenerateOutput.Api.Services;
using GenerateOutput.Models;

namespace GenerateOutput.Test
{
    public class RPSDBProcessManagerTest
    {
        Mock<ILogger<IRPSDBProcessManager>> _loggerMock = new Mock<ILogger<IRPSDBProcessManager>>();

        [Fact]
        public void RPSDBProcessManagerTest_RPSDBProcessManagerExist()
        {
            RPSDBProcessManager objectUnderTest = new RPSDBProcessManager(_loggerMock.Object);
            Assert.NotNull(objectUnderTest);
        }

        [Fact]
        public void RPSDBProcessManagerTest_SaveToDB_With_data()
        {
            _loggerMock.Setup(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            RPSDBProcessManager objectUnderTest = new RPSDBProcessManager(_loggerMock.Object);
            bool status = objectUnderTest.SaveToDB("key1", getCompensations());
            Assert.True(status);
            _loggerMock.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
        }

        [Fact]
        public void RPSDBProcessManagerTest_SaveToDB_With_Empty_Data()
        {
            _loggerMock.Setup(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            RPSDBProcessManager objectUnderTest = new RPSDBProcessManager(_loggerMock.Object);
            bool status = objectUnderTest.SaveToDB("key1", new List<Compensation>());
            Assert.False(status);
            _loggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
        }
        private IList<Compensation> getCompensations()
        {
            var compensations = new List<Compensation>();
            for (var i=0; i<=5; i++)
                {
                    compensations.Add(new Compensation()
                    {
                        // TODO: Set DivCode and RegCode to compensation object
                        AccountId = "AccountId_"+i,
                        Code = "Code_"+i,
                        Value = "Value_"+i,
                        ComplyDate = "ComplyDate_"+i,
                        DivCode = "DivCode"+i,
                        RegCode = "RegCode"+i,
                        PostId = "DailyCyclePostID_"+i,
                        SourceType = "SourceType_"+i,
                        SourceCode = "SourceCode_"+i,
                        PlanCode = "PlanCode_"+i,
                        WireNumber = "WireNumber"+i
                    });
                }
            return compensations;
        }
    }
}