using Confluent.Kafka;

namespace Common.Lib.Kafka
{
    public interface IKafkaConfiguration
    {
        /// <summary>
        /// GetConsumerConfig : returns local consumer config or envoirnment specific kafka configuration
        /// </summary>
        /// <returns>ConsumerConfig : either local env or targetd env config</returns>
        ConsumerConfig ConsumerConfig { get; }
        ProducerConfig ProducerConfig {get;}
        string ParticipantTopicName {get;}
        string ErrorTopicName {get;}
        string PlanTopicName {get;}
        string ControllerTopicName {get;}
        string ParticipantAllocationsTopicName {get;}
    }
}
