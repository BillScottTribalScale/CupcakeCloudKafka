using System;
using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using Xunit;
using Common.Lib.Kafka;

namespace Common.Test
{
    public class KafkaPublisherTest
    {
        Mock<ILogger<KafkaPublisher>> _mockLogger;
        Mock<IProducer<string, string>> _mockProducer;

        public KafkaPublisherTest()
        {
            _mockLogger = new Mock<ILogger<KafkaPublisher>>();
            _mockProducer = new Mock<IProducer<string, string>>();
        }

        [Fact]
        public void KafkaPublisher_ObjectExists()
        {
            var objectUnderTest = new KafkaPublisher(_mockLogger.Object, _mockProducer.Object);
            Assert.NotNull(objectUnderTest);
        }

        [Fact]
        public void KafkaPublisher_SendMessage()
        {
            //Setup
            _mockProducer.Setup(x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), default(CancellationToken)));
            //Act
            var objectUnderTest = new KafkaPublisher(_mockLogger.Object, _mockProducer.Object);
            objectUnderTest.SendMessage("One", "Two", new Guid());
            //Verify
            _mockProducer.Verify(x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), default(CancellationToken)), Times.Once());
        }

        [Fact]
        public void KafkaPublisher_SendMessage_When_Exception()
        {
            //Setup
            _mockProducer.Setup(x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), default(CancellationToken)))
            .ThrowsAsync(new Exception());
            //Act
            var objectUnderTest = new KafkaPublisher(_mockLogger.Object, _mockProducer.Object);
            objectUnderTest.SendMessage(null, null, new Guid());
            //Verify
            _mockProducer.Verify(x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>(), default(CancellationToken)), Times.Once());
            _mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.AtLeastOnce());
        }
    }
}
