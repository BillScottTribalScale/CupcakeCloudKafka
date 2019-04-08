using System.IO;
using System.Json;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using FileProcessor.Api.Models;
using System;
using System.Text;

namespace FileProcessor.Api.Services
{
    public class FileManager : IFileManager
    {
        private ILogger<FileManager> _logger;
        private const string META_KEY_NAME = "fileProperties";
        private const string FILE_RULE_KEY_NAME = "fieldProperties";
        private const string PARTICPANTS_KEY_NAME = "Participants";
        private const string COMPENSATION_RULE_TYPE = "compensation";
        private const string RULE_TYPE_KEY_NAME = "datatype";
        private const string ACCOUNTID_KEYWORD = "accountid";

        public FileManager(ILogger<FileManager> logger)
        {
            _logger = logger;
        }

        public JsonArray GetFileRules(JsonObject context)
        {
            return context[FILE_RULE_KEY_NAME] as JsonArray;
        }
        public JsonObject GetMetaData(JsonObject context)
        {

            return context[META_KEY_NAME] as JsonObject;
        }

        public JsonArray GetCompensationFileRules(JsonObject context)
        {
            JsonArray jo = GetFileRules(context);
            JsonArray compRules = new JsonArray();
            foreach (dynamic q in jo)
            {
                if (q[RULE_TYPE_KEY_NAME] == COMPENSATION_RULE_TYPE)
                {
                    compRules.Add(q);
                }

            }
            return compRules;
        }

        public JsonArray GetParticipants(JsonObject context)
        {
            return (context)[PARTICPANTS_KEY_NAME] as JsonArray;
        }

        public string ReadFile(string filename)
        {
            return File.ReadAllText(filename);
        }

        public CompensationInvestment CreateCompensationParticipant(JsonObject fileMeta, JsonObject fileRule, JsonObject participant, Guid fileKey)
        {
            if (ParticipantHasInvestment(participant, fileRule["code"]))
            {
                CompensationInvestment compInvestment = new CompensationInvestment();
                compInvestment.AccountId = GetParticipantFieldValue(participant, ACCOUNTID_KEYWORD);
                compInvestment.Value = Decimal.Parse(GetParticipantFieldValue(participant, fileRule["code"]), new System.Globalization.CultureInfo("en-US"));
                compInvestment.Code = fileRule["specialnumericcode"];
                compInvestment.ComplyDate = fileMeta["complianceDate"];
                compInvestment.DivCode = "";
                compInvestment.RegCode = "";
                compInvestment.fileKey = fileKey;
                return compInvestment;
            }
            return null;
        }

        public string GetParticipantFieldValue(JsonObject participant, string compField)
        {
            var props = GetIncomingParticipant(participant).
                    properties.
                    FirstOrDefault(f => f.name == compField).value;

            return props.ToString(new System.Globalization.CultureInfo("en-US"));

        }

        public IncomingParticipant GetIncomingParticipant(JsonObject participant)
        {
            IncomingParticipant ip;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(IncomingParticipant));
            using (MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(participant.ToString())))
            {
                ip = serializer.ReadObject(ms) as IncomingParticipant;
            }

            return ip;
        }

        public bool ParticipantHasInvestment(JsonObject participant, string compField)
        {
            decimal d = 0;
            var props = GetIncomingParticipant(participant).properties
                 .FirstOrDefault(f => f.name == compField && Decimal.TryParse(f.value, out d) && d > 0);
            return props != null;
        }

        public List<CompensationInvestment> CreateCompensationInvestments(JsonObject fileMeta, JsonArray fileRules, JsonArray participants, Guid fileKey)
        {
            List<CompensationInvestment> investments = new List<CompensationInvestment>();
            foreach (JsonObject part in participants)
            {
                foreach (JsonObject rule in fileRules)
                {
                    CompensationInvestment compInv = CreateCompensationParticipant(fileMeta, rule, part, fileKey);
                    if (compInv != null)
                    {
                        investments.Add(compInv);
                        _logger.LogTrace("CreateCompensationParticipant - created: {0}", compInv);
                    }
                }
            }
            return investments;
        }

        public Guid FileKeyGenerator()
        {
            return Guid.NewGuid();
        }
    }
}
