using System.Runtime.Serialization;

namespace CalculationEngine.Api.Models
{
    [DataContract]
    public class CompensationInvenstment : InvestmentBase 
    {
        public CompensationInvenstment () { 
         InvestmentType = "compensation";
        }
        [DataMember] 
        public string Code {get; set;}
        [DataMember] 
        public decimal Value {get; set;}
        [DataMember] 
        public string ComplyDate {get; set;}
        [DataMember] 
        public string DivCode {get; set;}
        [DataMember]  
        public string RegCode {get; set;}

        
    }
}
