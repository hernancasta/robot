using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messaging
{
    internal sealed class DefaultMessageSubscriber : IMessageSubscriber
    {
        public Task SubscribeAsync<T>(string topic, Action<T> handler) where T : class, IMessage => Task.CompletedTask;
    }
}
