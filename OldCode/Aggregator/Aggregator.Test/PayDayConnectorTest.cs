using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using Aggregator.Api.Services;
using Common.Lib.Kafka;
using System.Collections.Generic;

namespace Aggregator.Test
{
    public class PayDayConnectorTest
    {
        Mock<IMessageHandler<IAllocationManager>> _allocationMsgHandler = new Mock<IMessageHandler<IAllocationManager>> ();
        Mock<IMessageHandler<IMetaManager>> _metaMsgHandler = new Mock<IMessageHandler<IMetaManager>> ();
       
       [Fact]
        public void PayDayConnectorTest_CallsConsumeMessage()
        {
            var configMock = new Mock<IKafkaConfiguration>();
            configMock.Setup(x=>x.ControllerTopicName).Returns("MetaTopic");
            configMock.Setup(x=>x.ParticipantAllocationsTopicName).Returns("Allocationtopic");
            var consumerMock = new Mock<IKafkaConsumer>();
            consumerMock.Setup(p => p.ProcessMessages(It.IsAny<CancellationToken>(), It.IsAny<IDictionary<string,IMessageHandler>>()));
            Mock<ILogger<PayDayConnector>> loggerMock = new Mock<ILogger<PayDayConnector>>();
            var objectUnderTest = new PayDayConnector(loggerMock.Object, consumerMock.Object, configMock.Object,_allocationMsgHandler.Object,_metaMsgHandler.Object);
            var ctn = new CancellationTokenSource(100);
            objectUnderTest.StartAsync(ctn.Token);
            consumerMock.Verify(p => p.ProcessMessages(It.IsAny<CancellationToken>(), It.IsAny<IDictionary<string,IMessageHandler>>()), Times.Exactly(1));
            configMock.Verify(x=>x.ControllerTopicName, Times.Once);
            configMock.Verify(x=>x.ParticipantAllocationsTopicName,Times.Once);
        }

        [Fact]
        public void PayDayConnectorTest_WhenCancellationRequested()
        {
            var configMock = new Mock<IKafkaConfiguration>();
            configMock.Setup(x=>x.ControllerTopicName).Returns("MetaTopic");
            configMock.Setup(x=>x.ParticipantAllocationsTopicName).Returns("Allocationtopic");
            var consumerMock = new Mock<IKafkaConsumer>();
             consumerMock.Setup(p => p.ProcessMessages(It.IsAny<CancellationToken>(), It.IsAny<IDictionary<string,IMessageHandler>>()));
            Mock<ILogger<PayDayConnector>> loggerMock = new Mock<ILogger<PayDayConnector>>();
            var objectUnderTest = new  PayDayConnector(loggerMock.Object, consumerMock.Object, configMock.Object,_allocationMsgHandler.Object,_metaMsgHandler.Object);
            var ctn = new CancellationToken();
            objectUnderTest.StopAsync(ctn);
            consumerMock.Verify(p => p.ProcessMessages(It.IsAny<CancellationToken>(), It.IsAny<IDictionary<string,IMessageHandler>>()), Times.Never);
            configMock.Verify(x=>x.ControllerTopicName, Times.Once);
            configMock.Verify(x=>x.ParticipantAllocationsTopicName,Times.Once);
        }
    }
}
