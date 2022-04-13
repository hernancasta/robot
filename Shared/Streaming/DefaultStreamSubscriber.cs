using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Streaming
{
    internal class DefaultStreamSubscriber : IStreamSubscriber
    {
        //public IEnumerable<T> Snapshot<T>(string topic) where T : class
        //{
        //    return new List<T>();
        //}

        public Task SubscribeAsync<T>(string topic, Action<T> handler) where T : class => Task.CompletedTask;


    }
}
