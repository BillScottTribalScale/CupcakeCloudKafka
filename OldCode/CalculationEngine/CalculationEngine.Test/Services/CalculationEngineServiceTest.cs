using System;
using System.Text.RegularExpressions;
using CalculationEngine.Api.Services;
using CalculationEngine.Test.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CalculationEngine.Test
{

    // accept a message
    // validate message type
    // if type = compensation validate if we can create an outgoing message
    // send output message to the kafka topic

    public class CalculationEngineServiceTest
    {
        Mock<ILogger<CalculationEngineService>> _loggerMock;
        Mock<IMessagePublisher> _publisher;

        public CalculationEngineServiceTest()
        {
            _loggerMock = new Mock<ILogger<CalculationEngineService>>();
            _publisher = new Mock<IMessagePublisher>();
        }

        [Fact]
        public void CalculationEngineService_ObjectExists()
        {
            var objectUnderTest = new CalculationEngineService(_loggerMock.Object, _publisher.Object);
            Assert.NotNull(objectUnderTest);
        }

        [Fact]
        public void CalculationEngineService_StartProcess_With_CompensationType()
        {
            //setup
            _publisher.Setup(p => p.SendParticipantAllocationsMessage(It.IsAny<string>())).Returns(true);
            //act            
            var objectUnderTest = new CalculationEngineService(_loggerMock.Object, _publisher.Object);
            bool result = objectUnderTest.StartProcess(TestHelper.CreateCompMessage());
            //Assert
            Assert.True(result);
            _publisher.Verify(x => x.SendParticipantAllocationsMessage(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void CalculationEngineService_StartProcess_With_No_CompensationType()
        {
            //setup
            _publisher.Setup(p => p.SendParticipantAllocationsMessage(It.IsAny<string>())).Returns(true);
            //act            
            var objectUnderTest = new CalculationEngineService(_loggerMock.Object, _publisher.Object);
            bool result = objectUnderTest.StartProcess(TestHelper.CreateBadMessage());
            //Assert
            Assert.False(result);
            _publisher.Verify(x => x.SendParticipantAllocationsMessage(It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void CalculationEngineService_StartProcess_When_Exception()
        {
            //setup
            _publisher.Setup(p => p.SendParticipantAllocationsMessage(It.IsAny<string>())).Returns(true);
            //act            
            var objectUnderTest = new CalculationEngineService(_loggerMock.Object, _publisher.Object);
            bool result = objectUnderTest.StartProcess("ExceptionMsg");
            //Assert
            Assert.False(result);
            _publisher.Verify(x => x.SendParticipantAllocationsMessage(It.IsAny<string>()), Times.Never());
        }
    }
}
