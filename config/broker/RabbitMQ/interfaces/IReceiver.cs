using System;

namespace config.broker.RabbitMQ.interfaces
{
    public interface IReceiver
    {
        void Connect(string connectionName);
        void Receive(string queueName, Action<string> func, bool isDurable);
        void ReceiverExchange(string queueName, string exchangeName, string routingKeyValue, Action<string> func, bool isDurable);
    }
}