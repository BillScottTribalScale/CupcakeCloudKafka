using System;
using System.Collections.Generic;
using CalculationEngine.Api.Models;
using CalculationEngine.Api.Services;
using CalculationEngine.Test.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using Xunit;
using Common.Lib.Kafka;

namespace CalculationEngine.Test
{

    // accept a message
    // validate message type
    // if type = compensation validate if we can create an outgoing message
    // send output message to the kafka topic
    public class MessagePublisherTest
    {
        Mock<ILogger<MessagePublisher>> _mockLogger;
        Mock<IKafkaPublisher> _mockKafkaPublisher;
        Mock<IKafkaConfiguration> _mockKafkaConfigHelper;
        public MessagePublisherTest()
        {
            _mockLogger = new Mock<ILogger<MessagePublisher>>();
            _mockKafkaPublisher = new Mock<IKafkaPublisher>();
            _mockKafkaConfigHelper = new Mock<IKafkaConfiguration>();
        }

        [Fact]
        public void MessagePublisher_CanCreate()
        {
            var objectUnderTest = new MessagePublisher(_mockLogger.Object, _mockKafkaConfigHelper.Object, _mockKafkaPublisher.Object);
            Assert.NotNull(objectUnderTest);
        }

        [Fact]
        public void MessagePublisher_SendParticipantMessage()
        {
            _mockKafkaConfigHelper.Setup(p => p.ParticipantAllocationsTopicName).Returns("payday_participant_v1");
            _mockKafkaPublisher.Setup(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()));

            var objectUnderTest = new MessagePublisher(_mockLogger.Object, _mockKafkaConfigHelper.Object, _mockKafkaPublisher.Object);

            bool success = objectUnderTest.SendParticipantAllocationsMessage(TestHelper.CreateCompMessage());
            Assert.True(success);
        }

        [Fact]
        public void MessagePublisher_SendParticipantMessage_When_Exception()
        {    //Setup
            _mockKafkaConfigHelper.Setup(p => p.ParticipantAllocationsTopicName).Returns("payday_participant_v1");
            _mockKafkaPublisher.Setup(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Throws(new Exception());
            //Act
            var objectUnderTest = new MessagePublisher(_mockLogger.Object, _mockKafkaConfigHelper.Object, _mockKafkaPublisher.Object);
            bool success = objectUnderTest.SendParticipantAllocationsMessage(TestHelper.CreateCompMessage());
            //Verify
            Assert.False(success);
            _mockKafkaConfigHelper.Verify(x => x.ParticipantAllocationsTopicName, Times.Once());
            _mockKafkaPublisher.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Once());
            _mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.AtLeastOnce());
        }

        [Fact]
        public void MessagePublisher_CanExtractFileKey()
        {
            Guid fileKey = Guid.Empty;
            _mockKafkaConfigHelper.Setup(p => p.ParticipantAllocationsTopicName).Returns("payday_participant_v1");
            _mockKafkaPublisher.Setup(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Callback<string, string, Guid>((c1, c2, c3) => { fileKey = c3; });

            var objectUnderTest = new MessagePublisher(_mockLogger.Object, _mockKafkaConfigHelper.Object, _mockKafkaPublisher.Object);
            objectUnderTest.SendParticipantAllocationsMessage(TestHelper.CreateCompMessage());

            Assert.True(fileKey != Guid.Empty);
        }

        [Fact]
        public void MessagePublisher_CanExtractFileKey_When_Exception()
        {
            Guid fileKey = Guid.Empty;
            var msg = TestHelper.CreateCompMessage();
            msg = msg.Replace("fileKey", "flk");
            _mockKafkaConfigHelper.Setup(p => p.ParticipantAllocationsTopicName).Returns("payday_participant_v1");
            _mockKafkaPublisher.Setup(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
            .Callback<string, string, Guid>((c1, c2, c3) => { fileKey = c3; });

            var objectUnderTest = new MessagePublisher(_mockLogger.Object, _mockKafkaConfigHelper.Object, _mockKafkaPublisher.Object);
            var result = objectUnderTest.SendParticipantAllocationsMessage(msg);

            Assert.True(result);
            _mockKafkaConfigHelper.Verify(x => x.ParticipantAllocationsTopicName, Times.Once());
            _mockKafkaPublisher.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Once());
            _mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.AtLeastOnce());

        }
    }
}
