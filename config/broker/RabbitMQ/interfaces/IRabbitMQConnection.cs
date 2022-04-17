using RabbitMQ.Client;

namespace config.broker.RabbitMQ.interfaces
{
    public interface IRabbitMQConnection
    {
        IConnection Connection(string connectionName);
        (IConnection, IModel) Connection(string connectionName, string queueName, bool isDurable);
        (IConnection, IModel) ConnectionExchange(string connectionName, string exchangeDeclare, string typeExchange, bool isDurable);
        IModel QueueDeclare(IConnection connection, string queueName, bool isDurable);
        IModel ExchangeDeclare(IConnection connection, string exchangeName, string typeExchange, bool isDurable);
    }
}