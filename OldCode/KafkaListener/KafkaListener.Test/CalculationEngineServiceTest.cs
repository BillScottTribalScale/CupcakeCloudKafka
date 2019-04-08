using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KafkaListener.Api.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using Moq.Protected;
using Xunit;

namespace KafkaListener.Test
{
    public class CalculationEngineServiceTest
    {
         [Fact]
         public void CalculationEngineServiceTest_PublishMessage_Success()
        {
            Mock<ILogger<CalculationEngineService>> loggerMock = new Mock<ILogger<CalculationEngineService>>();
            // Arrange
            var httpClient = getHttpMessageHandler(HttpStatusCode.OK);                     
            var objectUnderTest = new CalculationEngineService(httpClient, loggerMock.Object);
            
            //Act
            var actualValue  =  objectUnderTest.HandleMessage("blah", "blah").Result;
            Assert.True(actualValue);
            //verify
            loggerMock.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.Never());
        }

        [Fact(Skip="Skip now")]
        public void CalculationEngineServiceTest_PublishMessage_Failure()
        {
            Mock<ILogger<CalculationEngineService>> loggerMock = new Mock<ILogger<CalculationEngineService>>();
            // Arrange
            var httpClient = getHttpMessageHandler(HttpStatusCode.BadRequest);                     
            var objectUnderTest = new CalculationEngineService(httpClient, loggerMock.Object);
            //Act
            var actualValue  =  objectUnderTest.HandleMessage("blah", "blah").Result;
            //verify
            Assert.False(actualValue);
            loggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.AtLeastOnce());
        }
         
        [Fact]
        public void CalculationEngineServiceTest_PublishMessage_WhenException()
        {
            Mock<ILogger<CalculationEngineService>> loggerMock = new Mock<ILogger<CalculationEngineService>>();
            // Arrange
            var httpClient = getHttpMessageHandler(HttpStatusCode.BadGateway,true);
            var objectUnderTest = new CalculationEngineService(httpClient, loggerMock.Object);
            //Act
            var actualValue  =  objectUnderTest.HandleMessage("blah", "blah").Result;
            Assert.False(actualValue);
            //verify
            loggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<FormattedLogValues>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.AtLeastOnce());
        }

        private static HttpClient getHttpMessageHandler(HttpStatusCode statusCode, bool excludeUrl = false)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent("test content")
                });

            var httpClient = excludeUrl 
            ? new HttpClient(mockMessageHandler.Object) 
            : new HttpClient(mockMessageHandler.Object)  { BaseAddress = new Uri("http://test.com/") };  
            
            return httpClient;
        }
    }
}
