using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Common.Lib.Kafka;

namespace KafkaListener.Api.Services
{
    /// <summary>
    /// CalculationEngineService is responsible invoking method that is discovered via discovery process
    /// </summary>
    public class CalculationEngineService : IMessageHandler<ICalculationEngineService>
    {
        private const string SERVICE_URL = "process"; 
        private readonly ILogger<CalculationEngineService> _logger;        
        private readonly HttpClient _httpClient;

        public CalculationEngineService(HttpClient httpClient, ILogger<CalculationEngineService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// PublishMessage is used to call calculation engine method via service discovery
        /// </summary>
        /// <param name="messageContent">A message to post</param>
        /// <returns></returns>
        public async Task<bool> HandleMessage(string key, string message)
        {
            try
            {
                var returnStatus = await _httpClient.PostAsJsonAsync(SERVICE_URL, message);                
                _logger.LogTrace("CalculationEngineService: PostAsJsonAsync method called.  return value is {0}", returnStatus.StatusCode);               
                return returnStatus.IsSuccessStatusCode;
            }
            catch (Exception ex)    
            {
                _logger.LogCritical("CalculationEngineService: Error while Publising message. Details : {0}", ex);
            }
            return false;
        }
    }
}
