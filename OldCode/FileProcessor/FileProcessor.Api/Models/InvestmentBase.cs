using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace FileProcessor.Api.Models
{
    [DataContract]
    public class InvestmentBase
    {
        [DataMember]
        public string AccountId { get; set; }
        [DataMember]
        public string InvestmentType { get; set; }
        [DataMember]
        public Guid fileKey { get; set; }

        public string GenerateJson()
        {

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(CompensationInvestment));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, this);
                ms.Position = 0;
                StreamReader sr = new StreamReader(ms);
                var jInv = sr.ReadToEnd();
                ms.Close();
                return jInv;
            }
        }
    }
}
