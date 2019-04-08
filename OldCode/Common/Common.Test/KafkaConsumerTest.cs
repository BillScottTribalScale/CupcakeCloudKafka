using System;
using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using Xunit;
using Common.Lib.Kafka;

namespace Common.Test
{
    public class KafkaConsumerTest
    {
        Mock<ILogger<KafkaConsumer>> _loggerMock = new Mock<ILogger<KafkaConsumer>>();
        
        [Fact]
        public void KafkaConsumerTest_CanCreateConsumer()
        {
            var consumerMock = new Mock<IConsumer<string, string>>();
            var objectUnderTest = new KafkaConsumer(_loggerMock.Object, consumerMock.Object );
            Assert.NotNull(objectUnderTest);
        }

        [Fact]
        public void KafkaConsumerTest_ProcessMessages_Success()
        {
            var consumerMock = new Mock<IConsumer<string, string>>();
            var ctn = new CancellationTokenSource();
            var messageHandlerMock = new Mock<IMessageHandler>();
            var objectUnderTest = new KafkaConsumer(_loggerMock.Object, consumerMock.Object);
            var consumeRes = new ConsumeResult<string, string>()
            {
                Message = new Message<string, string> { Key = "key1", Value = "value1" },
                TopicPartitionOffset = new TopicPartitionOffset(new TopicPartition("t", new Partition()), new Offset(0))
            }; 

            consumerMock.Setup(x => x.Consume(TimeSpan.FromMilliseconds(100))).Returns(consumeRes).Callback(() => ctn.Cancel());
            consumerMock.Setup(x => x.Subscribe(It.IsAny<string>()));
            //ACTION
            objectUnderTest.ProcessMessages(ctn.Token, "TestTopic", messageHandlerMock.Object);

            //ASSERT
            //1. Verify consumer.subscribe has been called
            //2. verify on success next method is called
            consumerMock.Verify(x => x.Subscribe(It.IsAny<string>()), Times.Once);
            consumerMock.Verify(x => x.Consume(TimeSpan.FromMilliseconds(100)), Times.AtLeastOnce);
            messageHandlerMock.Verify(x => x.HandleMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
       
        [Fact()]
        public void KafkaConsumerTest_ProcessMessages_Success_When_No_Consumer_Results()
        {
            //Assemble
            var consumerMock = new Mock<IConsumer<string, string>>();
            var ctn = new CancellationTokenSource();
            var messageHandlerMock = new Mock<IMessageHandler>();
            var objectUnderTest = new KafkaConsumer(_loggerMock.Object, consumerMock.Object);
            var consumeRes = new ConsumeResult<string, string>(); 

            consumerMock.Setup(x => x.Consume(TimeSpan.FromMilliseconds(100))).Returns(consumeRes).Callback(() => ctn.Cancel());
            consumerMock.Setup(x => x.Subscribe(It.IsAny<string>()));
            //Action
            objectUnderTest.ProcessMessages(ctn.Token, "TestTopic", messageHandlerMock.Object);

            //Assert
            consumerMock.Verify(x => x.Subscribe(It.IsAny<string>()), Times.Once);
            consumerMock.Verify(x => x.Consume(TimeSpan.FromMilliseconds(100)), Times.AtLeastOnce);
            messageHandlerMock.Verify(x => x.HandleMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }


        [Fact]
        public void KafkaConsumerTest_ProcessMessages_Single()
        {
            var consumerMock = new Mock<IConsumer<string, string>>();
            var consumerResult = new ConsumeResult<string, string>() { Message = new Message<string, string> { Key = "key", Value = "value" } };
            consumerMock.Setup(p => p.Consume(It.IsAny<TimeSpan>())).Returns(consumerResult);
            var mockLogger = new Mock<ILogger<KafkaConsumer>>();
            mockLogger.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            var objectUnderTest = new KafkaConsumer(mockLogger.Object, consumerMock.Object );

            var ctn = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));
            objectUnderTest.ProcessMessages(ctn.Token, It.IsAny<string>(),It.IsAny<IMessageHandler>());

            consumerMock.Verify(x => x.Subscribe(It.IsAny<string>()), Times.Once());
            //mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.AtLeastOnce());
            //consumerMock.Verify(x => x.Consume(It.IsAny<TimeSpan>()),Times.Once());
        }

        [Fact]
        public void KafkaConsumerTest_ProcessMessages_Many()
        {
            var consumerMock = new Mock<IConsumer<string, string>>();
            consumerMock.Setup(p => p.Subscribe(It.IsAny<string>()));
            var result1 = new ConsumeResult<string, string>()
            {
                Message = new Message<string, string> { Key = "key1", Value = "value1" }
            ,
                TopicPartitionOffset = new TopicPartitionOffset(new TopicPartition("t", new Partition()), new Offset(0))
            };
            var result2 = new ConsumeResult<string, string>()
            {
                Message = new Message<string, string> { Key = "key2", Value = "value2" }
            ,
                TopicPartitionOffset = new TopicPartitionOffset(new TopicPartition("t", new Partition()), new Offset(1))
            };
            consumerMock.Setup(p => p.Consume(It.IsAny<TimeSpan>())).Returns(result1);
            consumerMock.Setup(p => p.Consume(It.IsAny<TimeSpan>())).Returns(result2);

            var objectUnderTest = new KafkaConsumer(_loggerMock.Object, consumerMock.Object );
            var ctn = new CancellationTokenSource(TimeSpan.FromMilliseconds(17));
            objectUnderTest.ProcessMessages(ctn.Token, It.IsAny<string>(), It.IsAny<IMessageHandler>());

            consumerMock.Verify(x => x.Subscribe(It.IsAny<string>()), Times.Once());
            //consumerMock.Verify(x => x.Consume(It.IsAny<TimeSpan>()),Times.Exactly(2));
        }

        [Fact]
        public void KafkaConsumerTest_ProcessMessages_WhenCancellationTokenRequested()
        {
            /// ARRANGE
            var ctn = new CancellationTokenSource();
            ctn.Cancel();

            var consumerMock = new Mock<IConsumer<string, string>>();
            consumerMock.Setup(p => p.Subscribe(It.IsAny<string>()));
            consumerMock.Setup(p => p.Consume(TimeSpan.FromMilliseconds(10))).Returns(new ConsumeResult<string, string>());
            var messageHandlerMock = new Mock<IMessageHandler>();
            var kafkaConsumerUnderTest = new KafkaConsumer(_loggerMock.Object, consumerMock.Object);

            /// ACTION
            kafkaConsumerUnderTest.ProcessMessages(ctn.Token, It.IsAny<string>(), messageHandlerMock.Object);

            /// ASSERT
            consumerMock.Verify(x => x.Subscribe(It.IsAny<string>()), Times.Once());
            consumerMock.Verify(x => x.Consume(It.IsAny<TimeSpan>()), Times.Never());
            messageHandlerMock.Verify(x => x.HandleMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void KafkaConsumerTest_When_Consumer_Is_Null_Exception()
        {
            var mockLogger = new Mock<ILogger<KafkaConsumer>>();
            mockLogger.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

            var kafkaConsumerUnderTest = new KafkaConsumer(mockLogger.Object, null);
            var ctn = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));

            kafkaConsumerUnderTest.ProcessMessages(ctn.Token, It.IsAny<string>(), It.IsAny<IMessageHandler>());
            mockLogger.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());

        }

        [Fact()]
        public void KafkaConsumerTest_ProcessMessages_When_Exception()
        {
            //Assemble
            var consumerMock = new Mock<IConsumer<string, string>>();
            var ctn = new CancellationTokenSource();
            var messageHandlerMock = new Mock<IMessageHandler>();
            var objectUnderTest = new KafkaConsumer(_loggerMock.Object, consumerMock.Object);
            var consumeRes = new ConsumeResult<string, string>(); 

            _loggerMock.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            consumerMock.Setup(x => x.Consume(TimeSpan.FromMilliseconds(100))).Throws(new Exception());
            consumerMock.Setup(x => x.Subscribe(It.IsAny<string>()));
            //Action
            objectUnderTest.ProcessMessages(ctn.Token, "TestTopic", messageHandlerMock.Object);

            //Assert
            consumerMock.Verify(x => x.Subscribe(It.IsAny<string>()), Times.Once);
            consumerMock.Verify(x => x.Consume(TimeSpan.FromMilliseconds(100)), Times.AtLeastOnce);
            _loggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.AtLeastOnce());
            messageHandlerMock.Verify(x => x.HandleMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());

        }

        [Fact]
        public void KafkaConsumerTest_When_Consumer_ConsumeException()
        {
            
            var mockLogger = new Mock<ILogger<KafkaConsumer>>();
            mockLogger.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            var consumerMock = new Mock<IConsumer<string, string>>();
            consumerMock.Setup(p => p.Subscribe(It.IsAny<string>()));
            var messageHandlerMock = new Mock<IMessageHandler>();
            var result1 = new ConsumeResult<byte[], byte[]>() { Message = new Message<byte[], byte[]>() };

            consumerMock.Setup(p => p.Consume(It.IsAny<TimeSpan>())).Throws(new ConsumeException(result1, new Error(ErrorCode.BrokerNotAvailable, "failure")));

            var objectUnderTest = new KafkaConsumer(mockLogger.Object, consumerMock.Object);
            var ctn = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));

            objectUnderTest.ProcessMessages(ctn.Token, It.IsAny<string>(), messageHandlerMock.Object);
            consumerMock.Verify(x => x.Subscribe(It.IsAny<string>()), Times.Once());
            consumerMock.Verify(x => x.Consume(It.IsAny<TimeSpan>()), Times.Once());
            mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Once());
            messageHandlerMock.Verify(x => x.HandleMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }
    }
}
