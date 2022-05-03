using Microsoft.Extensions.Hosting;
using Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Command
{
    internal abstract class CommandListenerService<T> : BackgroundService where T : Command
    {
        protected readonly IMessageSubscriber _messageSubscriber;
        private readonly string TOPIC;

        public CommandListenerService(IMessageSubscriber messageSubscriber, string topic)
        {
            _messageSubscriber = messageSubscriber;
            TOPIC = topic;
//            TOPIC = typeof(T).Name;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Subscribing to topic {TOPIC}");
            _messageSubscriber.SubscribeAsync<T>(TOPIC, message => {
                ExecuteCommand(message);
            });
            return Task.CompletedTask;
        }

        protected abstract void ExecuteCommand(T Message);

    }
}
