using System;
using System.Collections.Generic;
using System.Json;

namespace GenerateOutput.Test
{
    public static class TestHelper
    {
        internal static JsonObject getJsonObject()
        {
            return (JsonObject)JsonObject.Parse(System.IO.File.ReadAllText("TestInput.json"));
        }
    }
}