using System;

namespace Common.Lib.Kafka
{

    /// <summary>
    /// IKafkaPublisher interface for posting message to kafka
    /// </summary>
    public interface IKafkaPublisher
    {
        /// <summary>
        ///  This method is to send message to Kafka
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="topicName">Topic Name</param>
        /// <param name="fileKey"></param>
        /// <returns></returns>
        void SendMessage(string message, string topicName, Guid fileKey);
    }
}
