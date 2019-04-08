using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Lib.Kafka
{
    public class KafkaConfigureServices
    {
        public static KafkaConfigSettings InitializeKafka(IServiceCollection services, IConfiguration configuration)
        {
            KafkaConfigSettings kafkaConfigSettings = new KafkaConfigSettings();
            configuration.Bind("KafkaConfigSettings", kafkaConfigSettings);

            services.AddSingleton<KafkaConfigSettings>(kafkaConfigSettings);
            services.AddSingleton<IKafkaConfiguration, KafkaConfiguration>();
            services.AddSingleton<IKafkaPublisher, KafkaPublisher>();
            services.AddSingleton<IConsumer<string, string>, Consumer<string, string>>(
                s => new Consumer<string, string>(new KafkaConfiguration(kafkaConfigSettings).ConsumerConfig)
            );
            services.AddSingleton<IKafkaConsumer, KafkaConsumer>();
            services.AddSingleton<IProducer<string, string>>(s => new Producer<string, string>(
                 new KafkaConfiguration(kafkaConfigSettings).ProducerConfig)
                   );
            return kafkaConfigSettings;
        }
    }
}