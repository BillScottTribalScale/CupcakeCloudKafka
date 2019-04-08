using System;
using FileProcessor.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Diagnostics;
using System.Json;
using System.Collections.Generic;
using FileProcessor.Api.Models;
namespace FileProcessor.Test
{

    // accept a filename?
    // read file stream
    // deserialize stream
    public class FileProcessorServiceTest
    {
        private string _testFilePath = "PayDay.json";
        private string getJsonContent()
        {
            return System.IO.File.ReadAllText(_testFilePath);
        }
        private string getInvalidJsonContent()
        {

            return System.IO.File.ReadAllText("InvalidJsonFile.json");

        }
        Mock<ILogger<FileProcessorService>> _loggerMock = new Mock<ILogger<FileProcessorService>>();
        Mock<IFileManager> _reader = new Mock<IFileManager>();
        Mock<IMessageManager> _messageManager = new Mock<IMessageManager>();

        Mock<IMessagePublisher> _kafkaPublisher = new Mock<IMessagePublisher>();


        [Fact]
        public void FileProcessorService_ObjectExists()
        {
            var objectUnderTest = new FileProcessorService(_loggerMock.Object, _reader.Object, _messageManager.Object, _kafkaPublisher.Object);
        }

        [Fact]
        public void FileProcessorService_StartProcess_CreatesMeta_CallOnce()
        {
            _reader.Setup(p => p.ReadFile(It.IsAny<string>())).Returns(getJsonContent());
            _reader.Setup(p => p.GetMetaData(It.IsAny<JsonObject>())).Returns((JsonObject)JsonObject.Parse(@"{""test"":""test""}"));
            var objectUnderTest = new FileProcessorService(_loggerMock.Object, _reader.Object, _messageManager.Object, _kafkaPublisher.Object);
            objectUnderTest.StartProcess(getFileContent(_testFilePath));
            _reader.Verify(bar => bar.GetMetaData(It.IsAny<JsonObject>()), Times.Once());
        }

        [Fact]
        public void FileProcessorService_StartProcess_CreateFileRules()
        {
            _reader.Setup(p => p.ReadFile(It.IsAny<string>())).Returns(getJsonContent());
            _reader.Setup(p => p.GetMetaData(It.IsAny<JsonObject>())).Returns((JsonObject)JsonObject.Parse(@"{""test"":""test""}"));
            _reader.Setup(p => p.GetCompensationFileRules(It.IsAny<JsonObject>())).Returns(new JsonArray());
            var objectUnderTest = new FileProcessorService(_loggerMock.Object, _reader.Object, _messageManager.Object, _kafkaPublisher.Object);
            objectUnderTest.StartProcess(getFileContent(_testFilePath));
            _reader.Verify(bar => bar.GetCompensationFileRules(It.IsAny<JsonObject>()), Times.Once());
        }

        [Fact]
        public void FileProcessorService_StartProcess_CreateCompensationInvestments()
        {
            _reader.Setup(p => p.ReadFile(It.IsAny<string>())).Returns(getJsonContent());
            _reader.Setup(p => p.GetMetaData(It.IsAny<JsonObject>())).Returns(TestHelper.getFileInfo());
            _reader.Setup(p => p.GetFileRules(It.IsAny<JsonObject>())).Returns(TestHelper.getCompensationRules());
            _reader.Setup(p => p.GetParticipants(It.IsAny<JsonObject>())).Returns(TestHelper.GetParticipants());
            _reader.Setup(p => p.CreateCompensationInvestments(It.IsAny<JsonObject>(), It.IsAny<JsonArray>(), It.IsAny<JsonArray>(), It.IsAny<Guid>())).Returns(TestHelper.GenerateCompensationInvestments());
            var objectUnderTest = new FileProcessorService(_loggerMock.Object, _reader.Object, _messageManager.Object, _kafkaPublisher.Object);
            objectUnderTest.StartProcess(getFileContent(_testFilePath));
            // assert that fileManager read has been called.
            _reader.Verify(bar => bar.CreateCompensationInvestments
           (It.IsAny<JsonObject>(), It.IsAny<JsonArray>(), It.IsAny<JsonArray>(), It.IsAny<Guid>()), Times.Once());
        }

        private string getFileContent(string filePath)
        {
            return System.IO.File.ReadAllText(filePath);
        }

        [Fact]
        public void FileProcessorService_StartProcess_SendCompensationInvestments()
        {
            _reader.Setup(p => p.ReadFile(It.IsAny<string>())).Returns(getJsonContent());
            _reader.Setup(p => p.GetMetaData(It.IsAny<JsonObject>())).Returns(TestHelper.getFileInfo());
            _reader.Setup(p => p.GetFileRules(It.IsAny<JsonObject>())).Returns(TestHelper.getCompensationRules());
            _reader.Setup(p => p.GetParticipants(It.IsAny<JsonObject>())).Returns(TestHelper.GetParticipants());
            _reader.Setup(p => p.CreateCompensationInvestments(It.IsAny<JsonObject>(), It.IsAny<JsonArray>(), It.IsAny<JsonArray>(), It.IsAny<Guid>())).Returns(TestHelper.GenerateCompensationInvestments());
            _kafkaPublisher.Setup(p => p.SendParticipantMessages(It.IsAny<List<CompensationInvestment>>()));
            var objectUnderTest = new FileProcessorService(_loggerMock.Object, _reader.Object, _messageManager.Object, _kafkaPublisher.Object);
            objectUnderTest.StartProcess(getFileContent(_testFilePath));
            _kafkaPublisher.Verify(bar => bar.SendParticipantMessages(It.IsAny<List<CompensationInvestment>>()), Times.Once());
        }

        [Fact]
        public void FileProcessorService_StartProcess_SendMetaDataMessage()
        {
            _reader.Setup(p => p.ReadFile(It.IsAny<string>())).Returns(getJsonContent());
            _reader.Setup(p => p.GetMetaData(It.IsAny<JsonObject>())).Returns(TestHelper.getFileInfo());
            _reader.Setup(p => p.GetFileRules(It.IsAny<JsonObject>())).Returns(TestHelper.getCompensationRules());
            _reader.Setup(p => p.GetParticipants(It.IsAny<JsonObject>())).Returns(TestHelper.GetParticipants());
            _reader.Setup(p => p.CreateCompensationInvestments(It.IsAny<JsonObject>(), It.IsAny<JsonArray>(), It.IsAny<JsonArray>(), It.IsAny<Guid>())).Returns(TestHelper.GenerateCompensationInvestments());
            _kafkaPublisher.Setup(p => p.SendParticipantMessages(It.IsAny<List<CompensationInvestment>>()));
            _messageManager.Setup(p => p.CreateProcessSummary(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<JsonObject>(), It.IsAny<JsonArray>(), It.IsAny<JsonArray>(), It.IsAny<IList<CompensationInvestment>>(), It.IsAny<Guid>())).Returns(new ProcessSummary());
            _kafkaPublisher.Setup(p => p.SendMetaMessages(It.IsAny<JsonObject>())).Returns(true);
            var objectUnderTest = new FileProcessorService(_loggerMock.Object, _reader.Object, _messageManager.Object, _kafkaPublisher.Object);
            objectUnderTest.StartProcess(getFileContent(_testFilePath));
            _messageManager.Verify(messageMan => messageMan.CreateMetaMessage(It.IsAny<JsonObject>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            _kafkaPublisher.Verify(bar => bar.SendMetaMessages(It.IsAny<JsonObject>()), Times.Once());
        }

        [Fact]
        public void FileProcessorService_StartProcess_CreateParticipants()
        {
            _reader.Setup(p => p.ReadFile(It.IsAny<string>())).Returns(getJsonContent());
            _reader.Setup(p => p.GetMetaData(It.IsAny<JsonObject>())).Returns((JsonObject)JsonObject.Parse(@"{""test"":""test""}"));
            _reader.Setup(p => p.GetFileRules(It.IsAny<JsonObject>())).Returns(new JsonArray());
            _reader.Setup(p => p.GetParticipants(It.IsAny<JsonObject>())).Returns(new JsonArray());
            var objectUnderTest = new FileProcessorService(_loggerMock.Object, _reader.Object, _messageManager.Object, _kafkaPublisher.Object);
            objectUnderTest.StartProcess(getFileContent(_testFilePath));
            _reader.Verify(bar => bar.GetParticipants(It.IsAny<JsonObject>()), Times.Once());
        }

        [Fact]
        public void FileProcessorService_DeserializesFile()
        {
            string someJson = getFileContent(_testFilePath);
            var objJsonContent = (JsonObject)JsonObject.Parse(someJson);
            _reader.Setup(p => p.GetMetaData(It.IsAny<JsonObject>())); //.Returns(getJsonContent());        
            var objectUnderTest = new FileProcessorService(_loggerMock.Object, _reader.Object, _messageManager.Object, _kafkaPublisher.Object);
            var fileContents = objectUnderTest.StartProcess(someJson);
            _reader.Verify(bar => bar.GetMetaData(It.IsAny<JsonObject>()), Times.Once());
        }

        [Fact]
        public void FileProcessorService_CompensationExists()
        {
            _reader.Setup(p => p.ReadFile(It.IsAny<string>())).Returns(getJsonContent());
            var objectUnderTest = new FileProcessorService(_loggerMock.Object, _reader.Object, _messageManager.Object, _kafkaPublisher.Object);
            var fileContents = objectUnderTest.StartProcess(getFileContent(_testFilePath));
        }

        [Fact]
        public void FileProcessorService_StartProcess_CreatedfileKey()
        {
            _reader.Setup(p => p.ReadFile(It.IsAny<string>())).Returns(getJsonContent());
            var objectUnderTest = new FileProcessorService(_loggerMock.Object, _reader.Object, _messageManager.Object, _kafkaPublisher.Object);
            var fileKey = objectUnderTest.StartProcess(getFileContent(_testFilePath));
            //verify that StartProcess called FileKeyGenerator()
            _reader.Verify(bar => bar.FileKeyGenerator(), Times.Once());
        }
    }
}
