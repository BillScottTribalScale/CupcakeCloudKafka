using System;
using System.Json;
using System.Threading.Tasks;
using CalculationEngine.Api.Controllers;
using CalculationEngine.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CalculationEngine.Test.Services
{
    public class CalculationEngineApiTest
    {

        // create web api
        // accept a post of json object
        // "read message type" to determine path of execution (even though we know it's compensation)
        // -- for compompensation type message
        //       serialize the message and post to kafka topic.
        Mock<ICalculationEngineService> calcEngMock = new Mock<ICalculationEngineService>();
        Mock<ILogger<CalculationEngineController>> _loggerMock = new Mock<ILogger<CalculationEngineController>>();

        [Fact]
        public void CalculationEngine_ApiExists()
        {
            var underTestController = new CalculationEngineController(null, calcEngMock.Object);
            Assert.NotNull(underTestController);
        }

        [Fact]
        public async void CalculationEngine_AcceptsValidMessage_ReturnsOK()
        {

            OkResult expected = new OkResult();

            string messageContent = "FileName.json";
            calcEngMock.Setup(p => p.StartProcess(It.IsAny<string>())).Returns(true);
            var underTestController = new CalculationEngineController(_loggerMock.Object, calcEngMock.Object);

            var actual = underTestController.Process(messageContent);
            Assert.Equal(((OkResult)actual).StatusCode, expected.StatusCode);
        }

        [Fact]
        public async void CalculationEngine_RejectsInvalidMessage_ReturnsBadValue()
        {

            BadRequestResult expected = new BadRequestResult();

            string messageContent = "";
            var underTestController = new CalculationEngineController(_loggerMock.Object, calcEngMock.Object);

            var actual = underTestController.Process (messageContent);
            Assert.Equal (((BadRequestResult) actual).StatusCode, expected.StatusCode);
        }

        [Fact]
        public void CalculationEngineAPI_StartsProcess()
        {
            string messageContent = "FileName.json";
            calcEngMock.Setup(p => p.StartProcess(It.IsAny<string>())).Returns(true);
            var underTestController = new CalculationEngineController(_loggerMock.Object, calcEngMock.Object);

            var result = underTestController.Process (messageContent);

            calcEngMock.Verify(bar => bar.StartProcess(It.IsAny<string>()), Times.Once());

        }
    }
}
