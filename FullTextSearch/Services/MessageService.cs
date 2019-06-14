using FullTextSearch.Contracts;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullTextSearch.Services
{
    public class MessageService : IMessageService
    {
        private readonly IModel channel;

        public MessageService(IConfiguration config)
        {
            
        }

        public bool PublishMessage(string message)
        {
            throw new NotImplementedException();
        }

        public bool PublishMessage(byte[] message)
        {
            throw new NotImplementedException();
        }

    }
}
