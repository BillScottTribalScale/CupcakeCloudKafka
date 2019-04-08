using System;
using System.Globalization;
using Confluent.Kafka;

namespace Common.Lib.Kafka
{
    /// <summary>
    /// KafkaConfiguration class responsible for providing kafka ConsumerConfig
    /// based on configuration settings which is defined under appsettings.json file
    /// </summary>
    public class KafkaConfiguration : IKafkaConfiguration 
    {
        private readonly KafkaConfigSettings _kafkaConfigSettings;
        public KafkaConfiguration (KafkaConfigSettings kafkaConfigSettings) 
        {
            _kafkaConfigSettings = kafkaConfigSettings;
        }

        public ConsumerConfig ConsumerConfig => (_kafkaConfigSettings.IncludeSasl) ? getConsumerSASLConfiguration() : getConsumerLocalConfiguration();

        public ProducerConfig ProducerConfig =>  (_kafkaConfigSettings.IncludeSasl) ? getProducerSASLConfiguration() : getProducerLocalConfiguration();

        public string ParticipantTopicName => _kafkaConfigSettings.ParticipantTopicName;

        public string ErrorTopicName => _kafkaConfigSettings.ErrorTopicName;

        public string PlanTopicName => _kafkaConfigSettings.PlanTopicName;

        public string ControllerTopicName => _kafkaConfigSettings.ControllerTopicName;

        public string ParticipantAllocationsTopicName => _kafkaConfigSettings.ParticipantAllocationsTopicName;

        private ConsumerConfig getConsumerLocalConfiguration()
        {
             return new ConsumerConfig
            {
                BootstrapServers = _kafkaConfigSettings.LocalBootstrapServers,
                GroupId = _kafkaConfigSettings.GroupId,
                AutoOffsetReset =AutoOffsetResetType.Earliest                                       
            };
        }

        private ConsumerConfig getConsumerSASLConfiguration()
        {
            return new ConsumerConfig
            {
                BootstrapServers = _kafkaConfigSettings.DevBootstrapServers,
                GroupId = _kafkaConfigSettings.GroupId,
                ApiVersionRequest = _kafkaConfigSettings.ApiVersionRequest,
                BrokerVersionFallback = _kafkaConfigSettings.BrokerVersionFallback,
                ClientId = Environment.MachineName,
                StatisticsIntervalMs = _kafkaConfigSettings.StatisticsIntervalMs,
                SecurityProtocol = SecurityProtocolType.Sasl_Ssl,
                SaslMechanism = SaslMechanismType.Gssapi,
                SaslKerberosPrincipal = _kafkaConfigSettings.SaslKerberosPrincipal,
                SaslKerberosServiceName = _kafkaConfigSettings.SaslKerberosServiceName,                
                SaslKerberosKinitCmd = $"kinit -k -t {_kafkaConfigSettings.SaslKerberosKeytabPath}{_kafkaConfigSettings.SaslKerberosKeytab} {_kafkaConfigSettings.SaslKerberosPrincipal}",
                AutoOffsetReset =AutoOffsetResetType.Earliest,
                EnableAutoCommit = true,
                MetadataRequestTimeoutMs = _kafkaConfigSettings.MetadataRequestTimeoutMs                            
            };
        }

        private ProducerConfig getProducerLocalConfiguration()
        { 
            return new ProducerConfig
                {
                    BootstrapServers = _kafkaConfigSettings.LocalBootstrapServers,                
                    GroupId = _kafkaConfigSettings.GroupId        
                };
        }
        
        private ProducerConfig getProducerSASLConfiguration( ) 
        { 
            return new ProducerConfig
            {
                BootstrapServers = _kafkaConfigSettings.DevBootstrapServers,
                GroupId = _kafkaConfigSettings.GroupId,
                ApiVersionRequest = _kafkaConfigSettings.ApiVersionRequest,
                BrokerVersionFallback = _kafkaConfigSettings.BrokerVersionFallback,
                ClientId = Environment.MachineName,
                StatisticsIntervalMs = _kafkaConfigSettings.StatisticsIntervalMs,
                SecurityProtocol = SecurityProtocolType.Sasl_Ssl,
                SaslMechanism = SaslMechanismType.Gssapi,
                SaslKerberosPrincipal = _kafkaConfigSettings.SaslKerberosPrincipal,
                SaslKerberosServiceName = _kafkaConfigSettings.SaslKerberosServiceName,
                SaslKerberosKinitCmd = $"kinit -k -t {_kafkaConfigSettings.SaslKerberosKeytabPath}{_kafkaConfigSettings.SaslKerberosKeytab} {_kafkaConfigSettings.SaslKerberosPrincipal}",
                MessageTimeoutMs = _kafkaConfigSettings.MetadataRequestTimeoutMs               
            };
        }        
    }
}
