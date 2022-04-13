using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Streaming
{
    public interface IStreamSubscriber
    {
        Task SubscribeAsync<T>(string topic, Action<T> handler) where T : class;

       // IEnumerable<T> Snapshot<T>(string topic) where T : class;
    }
}
