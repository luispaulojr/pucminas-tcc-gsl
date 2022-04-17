using System;
using System.Text;
using config.broker.RabbitMQ.interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace config.broker.RabbitMQ.impl
{
    public class Receiver : IReceiver
    {
        private readonly IRabbitMQConnection rabbitMQConnection;
        private IConnection connection;

        public Receiver(IRabbitMQConnection rabbitMQConnection) => this.rabbitMQConnection = rabbitMQConnection;

        public void Connect(string connectionName)
        {
            this.connection = this.rabbitMQConnection.Connection(connectionName);
        }

        public void Receive(string queueName, Action<string> func, bool isDurable)
        {
            var channel = this.rabbitMQConnection.QueueDeclare(connection, queueName, isDurable);
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.Span;
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    Log.Information($"Recebendo da fila '{queueName}'");
                    func.Invoke(message);
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Erro no recebimento da mensagem. QueueName: '{queueName}'. Messagem: {ea.ToString()}. Erro: {e.Message}");
                    throw new Exception($"Erro no recebimento da mensagem. QueueName: '{queueName}'. Messagem: {ea.ToString()}. Erro: {e.Message}");
                }
                finally
                {
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        public void ReceiverExchange(string queueName, string exchangeName, string routingKeyValue, Action<string> func, bool isDurable)
        {
            var channel = this.rabbitMQConnection.QueueDeclare(connection, queueName, isDurable);

            var consumer = new EventingBasicConsumer(channel);

            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKeyValue);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.Span;
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                try
                {
                    Log.Information($"Recebendo da fila '{queueName}' com a routingKey: '{routingKey}'");
                    func.Invoke(message);
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Erro no recebimento da mensagem. ExchangeName: '{exchangeName}'. Messagem: {ea.ToString()}. Erro: {e.Message}");
                    throw new Exception($"Erro no recebimento da mensagem. ExchangeName: '{exchangeName}'. Messagem: {ea.ToString()}. Erro: {e.Message}");
                }
                finally
                {
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }
    }
}