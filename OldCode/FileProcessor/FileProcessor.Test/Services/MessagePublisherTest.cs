using FileProcessor.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Collections.Generic;
using FileProcessor.Api.Models;
using System;
using System.Json;
using Common.Lib.Kafka;

namespace FileProcessor.Test
{
    public class MessagePublisherTest
    {
        Mock<ILogger<MessagePublisher>> _loggerMock = new Mock<ILogger<MessagePublisher>>();
        Mock<IKafkaPublisher> _producer = new Mock<IKafkaPublisher>();
        Mock<IKafkaConfiguration> _configHelper = new Mock<IKafkaConfiguration>();

        [Fact]
        public void MessagePublisher_CanCreate()
        {
            var objectUnderTest = new MessagePublisher(_loggerMock.Object, _configHelper.Object, _producer.Object);
            Assert.NotNull(objectUnderTest);
        }

        [Fact]
        public void MessagePublisher_CanSendMessage()
        {
            _configHelper.Setup(p => p.ParticipantTopicName).Returns(It.IsAny<string>());
            _producer.Setup(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()));

            List<CompensationInvestment> invests = TestHelper.GenerateCompensationInvestments();
            var objectUnderTest = new MessagePublisher(_loggerMock.Object, _configHelper.Object, _producer.Object);
            objectUnderTest.SendParticipantMessages(invests);

            _configHelper.Verify(p => p.ParticipantTopicName, Times.Once());
            _producer.Verify(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(5));

        }

        [Fact]
        public void MessagePublisher_CanSendMetaMessage()
        {
            _configHelper.Setup(p => p.ControllerTopicName).Returns(It.IsAny<string>);
            _producer.Setup(p => p.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()));

            JsonObject fakeMessage = TestHelper.getFileInfo();
            fakeMessage.Add("fileID", Guid.NewGuid().ToString());
            fakeMessage.Add("participantAmountCount", 100);

            var objectUnderTest = new MessagePublisher(_loggerMock.Object, _configHelper.Object, _producer.Object);
            var res = objectUnderTest.SendMetaMessages(fakeMessage);
            Assert.True(res);

        }

    }
}
