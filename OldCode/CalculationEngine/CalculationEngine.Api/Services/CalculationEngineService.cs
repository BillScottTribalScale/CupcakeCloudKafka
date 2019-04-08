using System;
using System.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace CalculationEngine.Api.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CalculationEngineService : ICalculationEngineService
    {
        private readonly ILogger<CalculationEngineService> _logger;
        private readonly IMessagePublisher _publisher;

        private const string INVESTMENT_TYPE_NAME = "InvestmentType";
        private const string COMPENSATION_TYPE = "compensation";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="publisher"></param>
        public CalculationEngineService(ILogger<CalculationEngineService> logger, IMessagePublisher publisher)
        {
            _logger = logger;
            _publisher = publisher;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageContent"></param>
        /// <returns></returns>
        public bool StartProcess(string messageContent)
        {
            return dispatchMessage(messageContent);
        }

        private bool dispatchMessage(string messageContent)
        {
            switch (getInvestmentType(messageContent))
            {
                case COMPENSATION_TYPE:
                    return _publisher.SendParticipantAllocationsMessage(messageContent);
                default:
                    return false;
            }
        }

        private string getInvestmentType(string messageContent)
        {
            try
            {
                JsonValue investment = JsonObject.Parse(messageContent);
                return Regex.Replace(investment[INVESTMENT_TYPE_NAME].ToString(), @"[^\w]", "");
            }
            catch (Exception ex)
            {
                //TODO: what do we do in case of bad message
                return null;
            }

        }
    }
}
