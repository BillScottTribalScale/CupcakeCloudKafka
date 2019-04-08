using System;
using System.Collections.Generic;
using System.Json;
using FileProcessor.Api.Models;

namespace FileProcessor.Test
{
    public static class TestHelper
    {
        internal static JsonObject getJsonObject()
        {
            return (JsonObject)JsonObject.Parse(System.IO.File.ReadAllText("PayDay.json"));
        }
        internal static JsonObject getParticipantObject_withCompensation()
        {
            //System.Diagnostics.Debugger.Launch();            
            return (JsonObject)getJsonObject()["Participants"][0];
        }
        internal static JsonObject getFileInfo()
        {
            //System.Diagnostics.Debugger.Launch();            
            return (JsonObject)getJsonObject()["fileProperties"];
        }
        internal static JsonObject getCompensationRule()
        {
            //System.Diagnostics.Debugger.Launch(); 
            var jo = getJsonObject()["fieldProperties"];

            foreach (dynamic q in jo)
            {
                if (q["datatype"] == "compensation")
                    return q;
            }

            return null;
        }

        internal static JsonArray getCompensationRules()
        {
            //System.Diagnostics.Debugger.Launch(); 
            JsonArray jo = getJsonObject()["fieldProperties"] as JsonArray;
            JsonArray compRules = new JsonArray();
            foreach (dynamic q in jo)
            {
                if (q["datatype"] == "compensation")
                    compRules.Add(q);
            }

            return compRules;
        }

        internal static JsonArray GetParticipants()
        {
            return getJsonObject()["Participants"] as JsonArray;
        }

        internal static List<CompensationInvestment> GenerateCompensationInvestments()
        {
            List<CompensationInvestment> investments = new List<CompensationInvestment>();
            Guid g = Guid.NewGuid();
            for (int i = 0; i < 5; i++)
            {
                CompensationInvestment compInvestment = new CompensationInvestment();
                compInvestment.AccountId = "123456";
                compInvestment.Value = 111;
                compInvestment.Code = "nu07";
                compInvestment.ComplyDate = DateTime.UtcNow.ToString();
                compInvestment.DivCode = "";
                compInvestment.RegCode = "";
                compInvestment.fileKey = g;
                investments.Add(compInvestment);
            }
            return investments;
        }
    }
}
