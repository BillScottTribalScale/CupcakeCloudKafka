using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace CalculationEngine.Api.Models 
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class InvestmentBase 
    {
        [DataMember]
        public string AccountId { get; set; }

        [DataMember]
        public string InvestmentType { get; set; }

        [DataMember]
        public Guid fileKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GenerateJson () 
        {

            DataContractJsonSerializer serializer = new DataContractJsonSerializer (typeof (CompensationInvenstment));
            using (MemoryStream ms = new MemoryStream ()) 
            {
                serializer.WriteObject (ms, this);
                ms.Position = 0;
                StreamReader sr = new StreamReader (ms);
                var jInv = sr.ReadToEnd ();
                ms.Close ();
                return jInv;
            }
        }
    }
}
