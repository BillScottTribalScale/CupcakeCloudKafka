using System;
using System.IO;

namespace FileProcessor.Api.Models
{

    public class KafkaConfig
    {

        public string LocalBootstrapServers { get; set; }

        public string DevBootstrapServers { get; set; }

        public bool ApiVersionRequest { get; set; }

        public string GroupId { get; set; }

        public string BrokerVersionFallback { get; set; }

        public int StatisticsIntervalMs { get; set; }

        public string ErrorTopicName { get; set; }

        public string ParticipantTopicName { get; set; }
        public string ParticipantAllocationsTopicName { get; set; }

        public string ControllerTopicName { get; set; }

        public string PlanTopicName { get; set; }

        public string SaslKerberosPrincipal { get; set; }

        public string SaslKerberosServiceName { get; set; }

        public string SaslKerberosKeytab { get; set; }

        public string SaslKerberosKeytabPath { get; set; }

        public bool IncludeSasl { get; set; }

    }
}