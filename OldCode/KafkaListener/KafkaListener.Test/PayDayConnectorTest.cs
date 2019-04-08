using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using KafkaListener.Api.Services;
using Common.Lib.Kafka;

namespace KafkaListener.Test
{
    public class PayDayConnectorTest
    {

        [Fact]
        public void PayDayConnectorTest_CallsProcessMessage()
        {
            var consumerMock = new Mock<IKafkaConsumer>();
            consumerMock.Setup(p => p.ProcessMessages(It.IsAny<CancellationToken>(), It.IsAny<string>(), It.IsAny<IMessageHandler>()));
            Mock<ILogger<PayDayConnector>> loggerMock = new Mock<ILogger<PayDayConnector>>();
            Mock<IMessageHandler<ICalculationEngineService>> calcMock = new Mock<IMessageHandler<ICalculationEngineService>>();
            PayDayConnector objectUnderTest = new PayDayConnector(loggerMock.Object, consumerMock.Object, calcMock.Object);
            var ctn = new CancellationTokenSource(100);
            objectUnderTest.StartAsync(ctn.Token);
            consumerMock.Verify(x => x.ProcessMessages(It.IsAny<CancellationToken>(), It.IsAny<string>(), It.IsAny<IMessageHandler>()), Times.AtLeastOnce);
        }
        [Fact]
        public void PayDayConnectorTest_WhenCancellationRequested()
        {
            var consumerMock = new Mock<IKafkaConsumer>();
            consumerMock.Setup(p => p.ProcessMessages(It.IsAny<CancellationToken>(), It.IsAny<string>(), It.IsAny<IMessageHandler>()));
            Mock<ILogger<PayDayConnector>> loggerMock = new Mock<ILogger<PayDayConnector>>();
            Mock<IMessageHandler<ICalculationEngineService>> calcMock = new Mock<IMessageHandler<ICalculationEngineService>>();
            PayDayConnector objectUnderTest = new PayDayConnector(loggerMock.Object, consumerMock.Object, calcMock.Object);
            var ctn = new CancellationToken();
            objectUnderTest.StopAsync(ctn);
            consumerMock.Verify(x => x.ProcessMessages(It.IsAny<CancellationToken>(), It.IsAny<string>(), It.IsAny<IMessageHandler>()), Times.Never());
        }
    }
}
