using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using config.broker.RabbitMQ.interfaces;
using config.constants;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;

namespace config.broker.RabbitMQ.impl
{
    public class Sender : ISender
    {

        private readonly IConfiguration configuration;
        private readonly IRabbitMQConnection rabbitMQConnection;

        public Sender(IConfiguration configuration, IRabbitMQConnection rabbitMQConnection)
        {
            this.configuration = configuration;
            this.rabbitMQConnection = rabbitMQConnection;
        }

        public void SendObj(string connectionName, string queueName, object obj, bool isDurable)
        {
            (var connection, var channel) = rabbitMQConnection.Connection(connectionName, queueName, isDurable);

            try
            {
                Send(channel, queueName, obj, isDurable);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro no envio da messagem. Nao foi possivel converter o objeto em JSON. QueueName: '{queueName}'. Objeto: {obj.ToString()}. Erro: {e.Message}");
                throw new Exception($"Erro no envio da messagem. Nao foi possivel converter o objeto em JSON. QueueName: '{queueName}'. Objeto: {obj.ToString()}. Erro: {e.Message}");
            }
            finally
            {
                CloseConnection(connection, channel);
            }
        }
        
        public void SendFanout(string connectionName, string exchangeName, object obj, bool isDurable)
        {
            (var connection, var channel) = rabbitMQConnection.ConnectionExchange(connectionName, exchangeName, ConfigurationConstant.ExchangeTypeFanout, isDurable);

            string message;
            try
            {
                message = JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro no parse da messagem. Nao foi possivel converter o objeto em JSON. Exchange: '{exchangeName}'. Objeto: {obj.ToString()}. Erro: {e.Message}");
                throw new Exception($"Erro no parse da messagem. Nao foi possivel converter o objeto em JSON. Exchange: '{exchangeName}'. Objeto: {obj.ToString()}. Erro: {e.Message}");
            }

            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: "",
                    basicProperties: channel.CreateBasicProperties(),
                    body: body
                );
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro no envio da mensagem. Exchange: '{exchangeName}'. Message: {message}. Erro: {e.Message}");
                throw new Exception($"Erro no envio da mensagem. Exchange: '{exchangeName}'. Message: {message}. Erro: {e.Message}");
            }

            finally
            {
                CloseConnection(connection, channel);
            }
        }

        public void SendLot<T>(string connectionName, string queueName, IEnumerable<T> message, bool isDurable, int size = 100)
        {
            (var connection, var channel) = rabbitMQConnection.Connection(connectionName, queueName, isDurable);

            try
            {
                var enumaration = message.Select((item, index) => new { index, item }).GroupBy(obj => obj.index / size);

                foreach (var items in enumaration)
                {
                    var lote = items.Select(obj => obj.item);
                    try
                    {
                        Send(channel, queueName, lote, isDurable);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, $"Erro no envio o lote. QueueName: '{queueName}'. Lote: {JsonConvert.SerializeObject(lote)}. Erro: {e.Message}");
                        throw new Exception($"Erro no envio o lote. QueueName: '{queueName}'. Lote: {JsonConvert.SerializeObject(lote)}. Erro: {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro no envio o lote. QueueName: '{queueName}'. Message: {JsonConvert.SerializeObject(message)}. Erro: {e.Message}");
                throw new Exception($"Erro no envio o lote. QueueName: '{queueName}'. Message: {JsonConvert.SerializeObject(message)}. Erro: {e.Message}");
            }
            finally
            {
                CloseConnection(connection, channel);
            }
        }

        private void Send(IModel channel, string queueName, object obj, bool isDurable)
        {
            string message;
            try
            {
                message = JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro no parse da mensagem. Nao foi possivel converter o objeto em JSON. QueueName: '{queueName}'. Objeto: {obj.ToString()}. Erro: {e.Message}");
                throw new Exception($"Erro no parse da mensagem. Nao foi possivel converter o objeto em JSON. QueueName: '{queueName}'. Objeto: {obj.ToString()}. Erro: {e.Message}");
            }

            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(
                    exchange: "",
                    routingKey: queueName,
                    basicProperties: channel.CreateBasicProperties(),
                    body: body
                );
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro no envio da mensagem. QueueName: '{queueName}'. Message: {message}. Erro: {e.Message}");
                throw new Exception($"Erro no envio da mensagem. QueueName: '{queueName}'. Message: {message}. Erro: {e.Message}");
            }
        }

        private void CloseConnection(IConnection connection, global::RabbitMQ.Client.IModel channel)
        {
            channel.Close();
            connection.Close();
        }
    }
}