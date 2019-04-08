using System.Json;
using System.Collections.Generic;
using FileProcessor.Api.Models;
using System;

namespace FileProcessor.Api.Services
{

    public interface IMessagePublisher
    {
        void SendParticipantMessages(List<CompensationInvestment> investments);
        bool SendMetaMessages(JsonObject metaMessage);
    }

}