using System;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KafkaListener.Api.Services;
using KafkaListener.Api.Controllers;
using Common.Lib.Kafka;

namespace KafkaListener.Test
{
    public class KafkaListenerControllerTest
    {
        private Mock<ILogger<KafkaListenerController>> _loggerMock = new Mock<ILogger<KafkaListenerController>>();
        private Mock<IKafkaConsumer> _consumer = new Mock<IKafkaConsumer>();
    }
}
