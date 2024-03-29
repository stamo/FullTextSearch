﻿using FullTextSearch.Core.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileProcessorService
{
    public class ProcessorHostedService : IHostedService
    {
        private readonly IApplicationLifetime appLifetime;
        private readonly ILogger logger;
        private readonly IHostingEnvironment environment;
        private readonly IConfiguration configuration;
        private readonly IServiceProvider serviceProvider;
        private IConnection connection;
        private IModel channel;
        private string queueName;

        private readonly IConsoleTaskRecieverService consoleTaskReciever;

        public ProcessorHostedService(
            IConfiguration configuration,
            IHostingEnvironment environment,
            ILogger<ProcessorHostedService> logger,
            IApplicationLifetime appLifetime,
            IServiceProvider serviceProvider,
            IConsoleTaskRecieverService consoleTaskReciever)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.appLifetime = appLifetime;
            this.environment = environment;
            this.serviceProvider = serviceProvider;
            this.consoleTaskReciever = consoleTaskReciever;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("StartAsync method called.");

            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;

        }

        private void OnStarted()
        {
            logger.LogInformation("OnStarted method called.");

            try
            {
                string hostName = configuration.GetValue<string>("MessageQueue:HostName");
                queueName = configuration.GetValue<string>("MessageQueue:QueueName");
                string username = configuration.GetValue<string>("MessageQueue:UserName");
                string password = configuration.GetValue<string>("MessageQueue:Password");

                var factory = new ConnectionFactory() { HostName = hostName, UserName = username, Password = password };

                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += MessageReceived;

                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        private void OnStopping()
        {
            logger.LogInformation("OnStopping method called.");

            try
            {
                // Освобождават се ресурсите свързани с клиента
                channel.Dispose();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

        }

        private void OnStopped()
        {
            logger.LogInformation("OnStopped method called.");
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("StopAsync method called.");

            return Task.CompletedTask;
        }

        protected void MessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                // обработка на полученото съобщение
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                this.consoleTaskReciever.RecieveMessage(message);
            }
            catch (Exception ex)
            {
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: properties,
                                     body: ea.Body);

                this.logger.LogError(ex, "MessageReceived");
            }
        }
    }
}
