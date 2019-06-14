using FullTextSearch.Contracts;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTextSearch.Services
{
    public class MessageService : IMessageService
    {
        private IConnection connection;
        private readonly IModel channel;
        private readonly string queueName;

        public MessageService(IConfiguration config)
        {
            string hostName = config.GetValue<string>("MessageQueue:HostName");
            queueName = config.GetValue<string>("MessageQueue:QueueName");
            string username = config.GetValue<string>("MessageQueue:UserName");
            string password = config.GetValue<string>("MessageQueue:Password");

            var factory = new ConnectionFactory() { HostName = hostName, UserName = username, Password = password };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
        }

        public void PublishMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            PublishMessage(body);
        }

        public void PublishMessage(byte[] message)
        {
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: properties,
                                 body: message);
        }
    }
}
