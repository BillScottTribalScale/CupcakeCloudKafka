using System;

namespace Aggregator.Api.Services
{
    public interface IMetaManager
    {
        void ProcessMetaMessage(string key, string message);
    }


}

