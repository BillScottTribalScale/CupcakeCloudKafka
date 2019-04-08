using System.Json;
using System.Collections.Generic;
using CalculationEngine.Api.Models;
using System;

namespace CalculationEngine.Api.Services
{
    /// <summary>
    /// IMessagePublisher interface for sending allocation investment to kafka producer
    /// </summary>
    public interface IMessagePublisher 
    {
        /// <summary>
        /// This method is used to send the participant allocation message to kafka
        /// </summary>
        /// <param name="investment">participants investments</param>
        /// <returns>true/false</returns>
        bool SendParticipantAllocationsMessage(string investment);
    }
}
