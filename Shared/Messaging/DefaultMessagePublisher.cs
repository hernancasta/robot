using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messaging
{
    internal sealed class DefaultMessagePublisher : IMessagePublisher
    {
        public Task PublishAsync<T>(string topic, T message) where T : class, IMessage => Task.CompletedTask;
    }
}
