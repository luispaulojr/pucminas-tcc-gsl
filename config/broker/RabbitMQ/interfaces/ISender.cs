using System.Collections.Generic;

namespace config.broker.RabbitMQ.interfaces
{
    public interface ISender
    {
        void SendObj(string connectionName, string queueName, object obj, bool isDurable);
        void SendFanout(string connectionName, string exchangeName, object obj, bool isDurable);
        void SendLot<T>(string connectionName, string queueName, IEnumerable<T> message, bool isDurable, int size = 100);
    }
}