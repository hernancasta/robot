using Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Command
{
    internal class CommandHandler<T> : ICommandHandler<T> where T : Command
    {
        protected readonly IMessagePublisher _messagePublisher;
        private readonly string TOPIC;

        public CommandHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
            TOPIC = typeof(T).Name;
        }

        public async Task HandleAsync(T command) {
            await _messagePublisher.PublishAsync<T>(TOPIC, command);
        }

    }
}
