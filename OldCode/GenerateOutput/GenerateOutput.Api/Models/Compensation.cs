
using System.Collections.Generic;
using System.Text;

namespace GenerateOutput.Models
{
    public class Compensation
    {
        public string AccountId { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string ComplyDate { get; set; }
        public string DivCode { get; set; }
        public string RegCode { get; set; }
        public string PostId { get; set; }
        public string SourceType { get; set; }
        public string SourceCode { get; set; }
        public string PlanCode { get; set; }
        public string WireNumber { get; set; }

        private string getCompnesationAsString(bool includeWireNumber = true)
        {
            var data =  "\n\nAccountId:" + AccountId + ", Code:" + Code + ", Value:" + Value + ", ComplyDate:" + ComplyDate
            + ", DivCode:" + DivCode + ", RegCode:" + RegCode + ", PostId:" + PostId + ", SourceType:" + SourceType
            + ", SourceCode:" + SourceCode + ", PlanCode:" + PlanCode;

            if(includeWireNumber){
                data += ", WireNumber:" + WireNumber;
            }
            data +=  "\n\n";
            return data;
        }
        public static string GetCompensationsAsString(IList<Compensation> compensations, bool includeWireNumber = true)
        {
            StringBuilder data = new StringBuilder();
            foreach (var comp in compensations)
            {
                data.Append(comp.getCompnesationAsString(includeWireNumber)).AppendLine();
            }
            return data.ToString();
        }
    }
}
