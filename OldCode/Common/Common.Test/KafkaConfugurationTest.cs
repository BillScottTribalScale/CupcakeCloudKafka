using Common.Lib.Kafka;
using Xunit;

namespace Common.Test
{
    public class KafkaConfugurationTest
    {
        [Fact]
         public void KafkaConfuguration_GetLocalConsumerConfig() {
            var configSettings = new KafkaConfigSettings() { SaslKerberosPrincipal="blah", GroupId="group", ApiVersionRequest=false, IncludeSasl=false  };
            var objectUnderTest = new KafkaConfiguration (configSettings);
            var config  = objectUnderTest.ConsumerConfig;
            Assert.Null(config.SaslKerberosPrincipal);
         }
         
         [Fact]
         public void KafkaConfuguration_GetSASLConsumerConfig() {
            var configSettings = new KafkaConfigSettings() { SaslKerberosPrincipal="blah",  GroupId="group", ApiVersionRequest=false, IncludeSasl=true};
            var objectUnderTest = new KafkaConfiguration (configSettings);
            var config  = objectUnderTest.ConsumerConfig;
            Assert.NotNull(config.SaslKerberosPrincipal);
        }

        [Fact]
         public void KafkaConfuguration_GetLocalCProducerConfig() {
            var configSettings = new KafkaConfigSettings() { SaslKerberosPrincipal="blah", GroupId="group", ApiVersionRequest=false, IncludeSasl=false  };
            var objectUnderTest = new KafkaConfiguration (configSettings);
            var config  = objectUnderTest.ProducerConfig;
            Assert.Null(config.SaslKerberosPrincipal);
         }
         
         [Fact]
         public void KafkaConfuguration_GetSASLProducerConfig() {
            var configSettings = new KafkaConfigSettings() { SaslKerberosPrincipal="blah",  GroupId="group", ApiVersionRequest=false, IncludeSasl=true};
            var objectUnderTest = new KafkaConfiguration (configSettings);
            var config  = objectUnderTest.ProducerConfig;
            Assert.NotNull(config.SaslKerberosPrincipal);
        }

        [Fact]
         public void KafkaConfuguration_GetParticipantTopicName() {
            var configSettings = new KafkaConfigSettings() { SaslKerberosPrincipal="blah",  GroupId="group", ParticipantTopicName="test"};
            var objectUnderTest = new KafkaConfiguration (configSettings);
            Assert.True(objectUnderTest.ParticipantTopicName == "test");
        }

        [Fact]
         public void KafkaConfuguration_GetErrorTopicName() {
            var configSettings = new KafkaConfigSettings() { SaslKerberosPrincipal="blah",  GroupId="group", ErrorTopicName = "test"};
            var objectUnderTest = new KafkaConfiguration (configSettings);
            Assert.True(objectUnderTest.ErrorTopicName == "test");
        }

        [Fact]
         public void KafkaConfuguration_GetPlanTopicName() {
            var configSettings = new KafkaConfigSettings() { SaslKerberosPrincipal="blah",  GroupId="group", PlanTopicName = "test"};
            var objectUnderTest = new KafkaConfiguration (configSettings);
            Assert.True(objectUnderTest.PlanTopicName == "test");
        }

        [Fact]
         public void KafkaConfuguration_GetControllerTopicName() {
            var configSettings = new KafkaConfigSettings() { SaslKerberosPrincipal="blah",  GroupId="group", ControllerTopicName="test"};
            var objectUnderTest = new KafkaConfiguration (configSettings);
            Assert.True(objectUnderTest.ControllerTopicName == "test");
        }

        [Fact]
         public void KafkaConfuguration_GetParticipantAllocationsTopicName() {
            var configSettings = new KafkaConfigSettings() { SaslKerberosPrincipal="blah",  GroupId="group", ParticipantAllocationsTopicName="test"};
            var objectUnderTest = new KafkaConfiguration (configSettings);
            var config  = objectUnderTest.ProducerConfig;
           Assert.True(objectUnderTest.ParticipantAllocationsTopicName == "test");
        }
    }
}
