using System;
using System.Json;
using FileProcessor.Api.Models;
using FileProcessor.Api.Controllers;
using FileProcessor.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FileProcessor.Test
{

    // accept a filename?
    // read file stream
    // deserialize stream
    // 

    public class FileProcessorAPITest
    {
        Mock<ILogger<FileProcessorController>> loggerMock = new Mock<ILogger<FileProcessorController>>();
        Mock<IFileProcessorService> fileProcMock = new Mock<IFileProcessorService>();
        Mock<IFileManager> fileManagerMock = new Mock<IFileManager>();

        [Fact]
        public void FileProcessorAPI_APIExists()
        {
            var underTestController = new FileProcessorController(loggerMock.Object, fileProcMock.Object, fileManagerMock.Object);
        }

        [Fact]
        public void FileProcessor_AcceptEncodedFileContent_ReturnsOK()
        {
            fileProcMock.Setup(s => s.StartProcess(It.IsAny<string>())).Returns(new ProcessSummary());
            var expected = new OkObjectResult(new object());
            string fileEncContent = System.IO.File.ReadAllText("./Resources/EncryptedSample.json");
            var objectUnderTest = new FileProcessorController(loggerMock.Object, fileProcMock.Object, fileManagerMock.Object);
            var result = objectUnderTest.Post(fileEncContent);
            Assert.Equal(((OkObjectResult)result).StatusCode, expected.StatusCode);
            fileProcMock.Verify(s => s.StartProcess(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void FileProcessor_InvalidContent_ReturnsBadRequest()
        {
            fileProcMock.Setup(s => s.StartProcess("testfile")).Returns(new ProcessSummary());
            var expected = new BadRequestObjectResult(new object());
            var objectUnderTest = new FileProcessorController(loggerMock.Object, fileProcMock.Object, fileManagerMock.Object);
            var result = objectUnderTest.Post("temp");
            Assert.Equal(((BadRequestObjectResult)result).StatusCode, expected.StatusCode);
        }

        [Fact]
        public void FileProcessorAPI_MissingFileName_DoesnotPerform_StartProcess()
        {
            string fileName = "./Resources/NonExistantFile.json";
            fileManagerMock.Setup(s => s.ReadFile(It.IsAny<string>()));
            fileProcMock.Setup(s => s.StartProcess(It.IsAny<string>())).Returns(new ProcessSummary());
            var underTestController = new FileProcessorController(loggerMock.Object, fileProcMock.Object, fileManagerMock.Object);
            var expected = new BadRequestResult();
            var actual = underTestController.Post(fileName);
            fileProcMock.Verify(s => s.StartProcess(It.IsAny<string>()), Times.Never);
        }

    }

}