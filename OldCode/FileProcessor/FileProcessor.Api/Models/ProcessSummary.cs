using System;

namespace FileProcessor.Api.Models
{


    public class ProcessSummary
    {
        public string FileName {get;set;}
        public string FileKey {get;set;}
        public int RulesDecoded {get;set;}
        public int Participants {get;set;}
        public int InvestmentsCreated {get; set;}
        public DateTime StartTime {get;set;}
        public DateTime EndTime {get;set;}       
    }
}