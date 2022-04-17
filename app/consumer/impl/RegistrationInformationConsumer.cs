using System;
using app.consumer.interfaces;
using config.broker.RabbitMQ.interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace app.consumer.impl
{
    public class RegistrationInformationConsumer : IRegistrationInformationConsumer
    {
        private readonly IReceiver receiver;
        private readonly IConfiguration configuration;

        public RegistrationInformationConsumer(IReceiver receiver, IConfiguration configuration)
        {
            this.receiver = receiver;
            this.configuration = configuration;
        }

        public void Init()
        {
            receiver.Connect("pucminas");
            receiver.Receive(queueName: configuration["RabbitMQ:pucminas:queues:queueRegistrationInformation"], func: this.exibeMessage, isDurable: true);
        }

        private void exibeMessage(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}