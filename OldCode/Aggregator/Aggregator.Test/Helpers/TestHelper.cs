using System;
using System.Collections.Generic;
using System.Json;
using Moq.Language.Flow;
//using FileProcessor.Api.Models;

namespace Aggregator.Test
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
    }
    public static class MoqExtensions
    {
        public static void ReturnsInOrder<T, TResult>(this ISetup<T, TResult> setup, Delegate d,
            params TResult[] results) where T : class  {
            setup.Returns(new Queue<TResult>(results).Dequeue).Callback(d);
        }
    }
}