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
    public class MetaManagerTest
    {

        Mock<ILogger<IMetaManager>> _metaLoggerMock = new Mock<ILogger<IMetaManager>>();
        Mock<IAggregationManager> _aggregationManagerMock = new Mock<IAggregationManager>();
        Mock<IKafkaConfiguration> _configMock = new Mock<IKafkaConfiguration>();

        [Fact]
        public void MetaManagerTest_HandleMessage()
        {
            // Arrange
            _metaLoggerMock.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _aggregationManagerMock.Setup(x => x.AddToMetaRepo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.GetMetaCount(It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()));

            String badMessage = "";

            MetaManager objectUnderTest = new MetaManager(_metaLoggerMock.Object, _configMock.Object, _aggregationManagerMock.Object);

            // Action
            var result = objectUnderTest.HandleMessage(Guid.NewGuid().ToString(), "Blah blah");

            // Assert
            _metaLoggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Never());
            _aggregationManagerMock.Verify(x => x.AddToMetaRepo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.GetMetaCount(It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void MetaManagerTest_HandleMessage_ExceptionWhenKeyIsNull()
        {
            // Arrange
            _metaLoggerMock.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _aggregationManagerMock.Setup(x => x.AddToMetaRepo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.GetMetaCount(It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()));

            String badMessage = "";

            MetaManager objectUnderTest = new MetaManager(_metaLoggerMock.Object, _configMock.Object, _aggregationManagerMock.Object);

            // Action
            var result = objectUnderTest.HandleMessage(null, "Blah blah");

            // Assert
            _metaLoggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.AddToMetaRepo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never());
            _aggregationManagerMock.Verify(x => x.GetMetaCount(It.IsAny<string>()), Times.Never());
            _aggregationManagerMock.Verify(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void MetaManagerTest_HandleMessage_ExceptionWhenAddingToMetaRepo()
        {
            // Arrange
            _metaLoggerMock.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _aggregationManagerMock.Setup(x => x.AddToMetaRepo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Throws(new Exception());
            _aggregationManagerMock.Setup(x => x.GetMetaCount(It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()));

            String badMessage = "";

            MetaManager objectUnderTest = new MetaManager(_metaLoggerMock.Object, _configMock.Object, _aggregationManagerMock.Object);

            // Action
            var result = objectUnderTest.HandleMessage("247d7eb3-2fbe-4b3c-84ef-f3580f5c6331", "Blah blah");

            // Assert
            _metaLoggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.GetMetaCount(It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.AddToMetaRepo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void MetaManagerTest_HandleMessage_ExceptionWhenGetMetaCount()
        {
            // Arrange
            _metaLoggerMock.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _aggregationManagerMock.Setup(x => x.AddToMetaRepo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.GetMetaCount(It.IsAny<string>())).Throws(new Exception());
            _aggregationManagerMock.Setup(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()));

            String badMessage = "";

            MetaManager objectUnderTest = new MetaManager(_metaLoggerMock.Object, _configMock.Object, _aggregationManagerMock.Object);

            // Action
            var result = objectUnderTest.HandleMessage("247d7eb3-2fbe-4b3c-84ef-f3580f5c6331", "Blah blah");

            // Assert
            _metaLoggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.GetMetaCount(It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.AddToMetaRepo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never());
            _aggregationManagerMock.Verify(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void MetaManagerTest_HandleMessage_ExceptionWhenEnrichAndSendMessage()
        {
            // Arrange
            _metaLoggerMock.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _aggregationManagerMock.Setup(x => x.AddToMetaRepo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.GetMetaCount(It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>())).Throws(new Exception());

            String badMessage = "";

            MetaManager objectUnderTest = new MetaManager(_metaLoggerMock.Object, _configMock.Object, _aggregationManagerMock.Object);

            // Action
            var result = objectUnderTest.HandleMessage("247d7eb3-2fbe-4b3c-84ef-f3580f5c6331", "Blah blah");

            // Assert
            _metaLoggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.AddToMetaRepo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.GetMetaCount(It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once());
        }
    }
}