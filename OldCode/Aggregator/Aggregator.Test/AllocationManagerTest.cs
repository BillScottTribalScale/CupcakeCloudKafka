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
    public class AllocationManagerTest
    {
        Mock<ILogger<IAllocationManager>> _allocationLoggerMock = new Mock<ILogger<IAllocationManager>>();
        Mock<IAggregationManager> _aggregationManagerMock = new Mock<IAggregationManager>();
        Mock<IKafkaConfiguration> _configMock = new Mock<IKafkaConfiguration>();

        [Fact]
        public void AllocationManagerTest_HandleMessage_Success()
        {
            // Arrange
            _allocationLoggerMock.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _aggregationManagerMock.Setup(x => x.AddToAllocationRepo(It.IsAny<string>(), It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.GetMetaCountByKey(It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()));

            String badMessage = "";

            AllocationManager objectUnderTest = new AllocationManager(_allocationLoggerMock.Object, _configMock.Object, _aggregationManagerMock.Object);

            // Action
            var result = objectUnderTest.HandleMessage(Guid.NewGuid().ToString(), "Blah blah");

            // Assert
            _allocationLoggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Never());
            _aggregationManagerMock.Verify(x => x.AddToAllocationRepo(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.GetMetaCountByKey(It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void AllocationManagerTest_HandleMessage_ExceptionWhenKeyIsNull()
        {
            // Arrange
            _allocationLoggerMock.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _aggregationManagerMock.Setup(x => x.AddToAllocationRepo(It.IsAny<string>(), It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.GetMetaCountByKey(It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()));

            AllocationManager objectUnderTest = new AllocationManager(_allocationLoggerMock.Object, _configMock.Object, _aggregationManagerMock.Object);

            // Action
            var result = objectUnderTest.HandleMessage(null, "Blah blah");

            // Assert
            _allocationLoggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.AddToAllocationRepo(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            _aggregationManagerMock.Verify(x => x.GetMetaCountByKey(It.IsAny<string>()), Times.Never());
            _aggregationManagerMock.Verify(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void AllocationManagerTest_HandleMessage_ExceptionWhenAddingToAllocationRepo()
        {
            // Arrange
            _allocationLoggerMock.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _aggregationManagerMock.Setup(x => x.AddToAllocationRepo(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            _aggregationManagerMock.Setup(x => x.GetMetaCountByKey(It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()));

            AllocationManager objectUnderTest = new AllocationManager(_allocationLoggerMock.Object, _configMock.Object, _aggregationManagerMock.Object);

            // Action
            var result = objectUnderTest.HandleMessage("247d7eb3-2fbe-4b3c-84ef-f3580f5c6331", "Blah blah");

            // Assert
            _allocationLoggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.AddToAllocationRepo(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.GetMetaCountByKey(It.IsAny<string>()), Times.Never());
            _aggregationManagerMock.Verify(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void AllocationManagerTest_HandleMessage_ExceptionWhenGetMetaCountByKey()
        {
            // Arrange
            _allocationLoggerMock.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _aggregationManagerMock.Setup(x => x.AddToAllocationRepo(It.IsAny<string>(), It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.GetMetaCountByKey(It.IsAny<string>())).Throws(new Exception());
            _aggregationManagerMock.Setup(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()));

            AllocationManager objectUnderTest = new AllocationManager(_allocationLoggerMock.Object, _configMock.Object, _aggregationManagerMock.Object);

            // Action
            var result = objectUnderTest.HandleMessage("247d7eb3-2fbe-4b3c-84ef-f3580f5c6331", "Blah blah");

            // Assert
            _allocationLoggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.AddToAllocationRepo(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.GetMetaCountByKey(It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void AllocationManagerTest_HandleMessage_ExceptionWhenEnrichAndSendMessage()
        {
            // Arrange
            _allocationLoggerMock.Setup(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            _aggregationManagerMock.Setup(x => x.AddToAllocationRepo(It.IsAny<string>(), It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.GetMetaCountByKey(It.IsAny<string>()));
            _aggregationManagerMock.Setup(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>())).Throws(new Exception());

            AllocationManager objectUnderTest = new AllocationManager(_allocationLoggerMock.Object, _configMock.Object, _aggregationManagerMock.Object);

            // Action
            var result = objectUnderTest.HandleMessage("247d7eb3-2fbe-4b3c-84ef-f3580f5c6331", "Blah blah");

            // Assert
            _allocationLoggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.AddToAllocationRepo(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.GetMetaCountByKey(It.IsAny<string>()), Times.Once());
            _aggregationManagerMock.Verify(x => x.EnrichAndSendMessage(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once());
        }
    }
}