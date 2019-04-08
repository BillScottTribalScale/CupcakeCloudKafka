using System.Threading.Tasks;

namespace Common.Lib.Kafka
{
    public interface IMessageHandler
    {
        Task<bool> HandleMessage(string key, string message);
    }
    public interface IMessageHandler<T> : IMessageHandler
    {
        /// just  a placeholder
    }
}