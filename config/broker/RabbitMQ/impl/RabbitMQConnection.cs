using System;
using config.broker.RabbitMQ.interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Serilog;

namespace config.broker.RabbitMQ.impl
{
    public class RabbitMQConnection : IRabbitMQConnection
    {

        private readonly IConfiguration configuration;

        public RabbitMQConnection(IConfiguration configuration) => this.configuration = configuration;

        public IConnection Connection(string connectionName)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = configuration[$"RabbitMQ:{connectionName}:Hostname"],
                    UserName = configuration[$"RabbitMQ:{connectionName}:Username"],
                    Password = configuration[$"RabbitMQ:{connectionName}:Password"],
                    Port = int.Parse(configuration[$"RabbitMQ:{connectionName}:Port"])
                };

                var connection = factory.CreateConnection(configuration["AppInformation:name"]);
                Log.Information($"Conexao criada. ConnectionName: '{connectionName}'");

                return connection;
            }
            catch (Exception e)
            {
                Log.Error(e, $"Nao foi possivel criar a conexao com o RabbitMQ. ConnectionName: '{connectionName}'. Erro: {e.Message}");
                throw new Exception($"Nao foi possivel criar a conexao com o RabbitMQ. ConnectionName: '{connectionName}'. Erro: {e.Message}");
            }
        }

        public (IConnection, IModel) Connection(string connectionName, string queueName, bool isDurable)
        {
            var connection = Connection(connectionName);
            var channel = QueueDeclare(connection, queueName, isDurable);

            return (connection, channel);
        }

        public (IConnection, IModel) ConnectionExchange(string connectionName, string exchangeDeclare, string typeExchange, bool isDurable)
        {
            var connection = Connection(connectionName);
            var channel = ExchangeDeclare(connection, exchangeDeclare, typeExchange, isDurable);

            return (connection, channel);
        }

        public IModel ExchangeDeclare(IConnection connection, string exchangeName, string typeExchange, bool isDurable)
        {
            IModel channel;

            try
            {
                channel = connection.CreateModel();
                channel.ExchangeDeclare(
                    exchange: exchangeName,
                    type: typeExchange,
                    durable: isDurable,
                    autoDelete: false,
                    arguments: null
                );

                Log.Information($"Exchange declarada. ExchangeName: '{exchangeName}' com o tipo: '{typeExchange}'");
            }
            catch (Exception e)
            {
                Log.Error(e, $"Nao foi possivel declarar o exchange. ExchangeName: '{exchangeName}' com o tipo: '{typeExchange}'. Erro: {e.Message}");
                throw new Exception($"Nao foi possivel declarar o exchange. ExchangeName: '{exchangeName}' com o tipo: '{typeExchange}'. Erro: {e.Message}");
            }

            return channel;
        }

        public IModel QueueDeclare(IConnection connection, string queueName, bool isDurable)
        {
            IModel channel;

            try
            {
                channel = connection.CreateModel();

                channel.QueueDeclare(
                    queue: queueName,
                    durable: isDurable,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                Log.Information($"Queue declarada. QueueName: '{queueName}'");
            }
            catch (Exception e)
            {
                Log.Error(e, $"Não foi possivel declarar a queue. QueueName: '{queueName}'. Erro: {e.Message}");
                throw new Exception($"Não foi possivel declarar a queue. QueueName: '{queueName}'. Erro: {e.Message}");
            }

            return channel;
        }
       
        
    }
}