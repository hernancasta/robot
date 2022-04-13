using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Streaming
{
    internal class DefaultStreamPublisher : IStreamPublisher
    {
        public Task PublishAsync<T>(string topic, T data) where T : class => Task.CompletedTask;

    }
}
