using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StraddleCore.Configurations.Azure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleCore.Configurations.Azure
{
    public class AzureServiceBusQueueConfiguration : IAzureServiceBusQueueConfiguration
    {
        private readonly IConfiguration _configuration;
        private readonly IQueueClient _queueClient;

        private readonly ILogger<AzureServiceBusQueueConfiguration> _logger;

        public AzureServiceBusQueueConfiguration(IConfiguration configuration, ILogger<AzureServiceBusQueueConfiguration> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _queueClient = new QueueClient(_configuration["AzureServiceBusConfig:ConnectionString"], _configuration["AzureServiceBusConfig:QueueName"]);
        }

        public async Task<Task> SendMessageAsync(string message)
        {
            Message? encodedMessage = new Message(Encoding.UTF8.GetBytes(message));
            await _queueClient.SendAsync(encodedMessage);

            return Task.CompletedTask;
        }

        public async Task ReceiveMessageAsync()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _queueClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);
            await Task.CompletedTask; // Let the method run continuously
        }

        private async Task ProcessMessageAsync(Message message, CancellationToken token)
        {
            string? messageBody = Encoding.UTF8.GetString(message.Body);

            // The message can be processed here according to the needed requirement.
            _logger.LogInformation($"Received message: {messageBody}");

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogInformation($"Message handler encountered an exception: {exceptionReceivedEventArgs.Exception}");
            return Task.CompletedTask;
        }
    }
}
