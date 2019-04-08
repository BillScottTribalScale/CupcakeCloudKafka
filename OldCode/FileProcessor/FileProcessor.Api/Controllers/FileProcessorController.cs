using System;
using System.Json;
using System.Text;
using System.Threading.Tasks;
using FileProcessor.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FileProcessor.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileProcessorController : ControllerBase
    {

        private readonly ILogger<FileProcessorController> _logger;
        private readonly IFileProcessorService _fileProcessorService;
        private const string FILE_CONTENT_FILE_NAME_KEY = "PaydayFilename";
        private const string FILE_CONTENT_NAME_KEY = "PaydayFileContent";
        private const string FILE_CONTENT_ENV_KEY = "Environment";
        private IFileManager _fileManager;
        public FileProcessorController(ILogger<FileProcessorController> logger,
            IFileProcessorService fileProcessorService, IFileManager fileManager)
        {
            _logger = logger;
            _fileProcessorService = fileProcessorService;
            _fileManager = fileManager;
        }

        [HttpPost]
        public IActionResult Post([FromBody] string fileContent)
        {
            try
            {
                var objJsonContent = (JsonObject)JsonObject.Parse(fileContent);
                if (objJsonContent.TryGetValue(FILE_CONTENT_FILE_NAME_KEY, out JsonValue fileName)
                && objJsonContent.TryGetValue(FILE_CONTENT_NAME_KEY, out JsonValue encodedContent))
                {
                    string decodedContent = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Convert.FromBase64String(encodedContent))).ToString();
                    objJsonContent.TryGetValue(FILE_CONTENT_ENV_KEY, out JsonValue envName);

                    _logger.LogInformation("File received: {0}", fileName.ToString());
                    var startProcessResult = _fileProcessorService.StartProcess(decodedContent);
                    _logger.LogInformation("File processed: {0}", fileName.ToString());
                    return new OkObjectResult(new { Filename = fileName.ToString(), ReadDateTime = DateTime.Now, ProcessResult = startProcessResult });
                }
                else
                {
                    return new BadRequestObjectResult(new { Error = "Either file name key or encoded content key is missing", Filename = fileName?.ToString(), ReadDateTime = DateTime.Now });
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Error while receiving file : {0}. Details : {1}. ", fileContent, ex);
                return new BadRequestObjectResult(new { Error = "Invalid content : " + ex, ReadDateTime = DateTime.Now });
            }
        }
    }
}