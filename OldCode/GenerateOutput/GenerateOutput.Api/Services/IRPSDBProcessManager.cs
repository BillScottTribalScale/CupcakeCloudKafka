using System.Collections.Generic;
using GenerateOutput.Models;

namespace GenerateOutput.Api.Services
{
    public interface IRPSDBProcessManager
    {
        bool SaveToDB(string fileKey, IList<Compensation> compensations);
    }
}