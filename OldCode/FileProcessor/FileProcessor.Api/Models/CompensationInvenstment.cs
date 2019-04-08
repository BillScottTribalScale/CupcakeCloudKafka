using System.Runtime.Serialization;

namespace FileProcessor.Api.Models
{
    [DataContract]
    public class CompensationInvestment : InvestmentBase
    {
        public CompensationInvestment()
        {
            InvestmentType = "compensation";
        }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public decimal Value { get; set; }
        [DataMember]
        public string ComplyDate { get; set; }
        [DataMember]
        public string DivCode { get; set; }
        [DataMember]
        public string RegCode { get; set; }
    }
}
