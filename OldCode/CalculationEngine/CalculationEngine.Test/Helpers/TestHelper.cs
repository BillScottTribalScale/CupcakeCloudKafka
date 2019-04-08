using System;
using System.Collections.Generic;
using CalculationEngine.Api.Models;

namespace CalculationEngine.Test.Helpers
{
    public static class TestHelper
    {

        public static string CreateBadMessage()
        {
            CompensationInvenstment compInvestment = getCompensationInvestment();
            compInvestment.InvestmentType = "NonCompensation";
            return compInvestment.GenerateJson();
        }
        public static string CreateCompMessage()
        {
            CompensationInvenstment compInvestment = getCompensationInvestment();
            return compInvestment.GenerateJson();
        }

        private static CompensationInvenstment getCompensationInvestment()
        {
            CompensationInvenstment compInvestment = new CompensationInvenstment();
            compInvestment.AccountId = "123456";
            compInvestment.Value = 111;
            compInvestment.Code = "nu07";
            compInvestment.ComplyDate = DateTime.UtcNow.ToString();
            compInvestment.DivCode = "";
            compInvestment.RegCode = "";
            compInvestment.fileKey = Guid.NewGuid();
            return compInvestment;
        }
    }
}