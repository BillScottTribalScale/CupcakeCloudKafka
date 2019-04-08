using System.Json;
using FileProcessor.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Collections.Generic;
using FileProcessor.Api.Models;
using System;

namespace FileProcessor.Test.Models
{

    public class ModelTest
    {
        [Fact]
        private void Model_CanGenerateJSon()
        {
            CompensationInvestment compInv = new CompensationInvestment();
            compInv.AccountId = "123456";
            compInv.Code = "nu07";
            compInv.Value = 111;
            compInv.fileKey = Guid.NewGuid();
            compInv.ComplyDate = DateTime.UtcNow.ToString();
            //System.Diagnostics.Debugger.Launch();
            var compInvJson = compInv.GenerateJson();
        }
    }
}