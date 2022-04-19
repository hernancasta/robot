using Shared.Serialization;
using Shared.Streaming;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Redis.Streaming
{
    internal sealed class RedisStreamSubscriber : IStreamSubscriber
    {
        private readonly ISubscriber _subscriber;
        private readonly ISerializer _serializer;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisStreamSubscriber(IConnectionMultiplexer connectionMultiplexer, ISerializer serializer)
        {
            _subscriber = connectionMultiplexer.GetSubscriber();
            _serializer = serializer;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public Task SubscribeAsync<T>(string topic, Action<T> handler) where T : class => _subscriber.SubscribeAsync(topic, (_, data) => {
            try
            {
                var payload = _serializer.Deserialize<T>(data);
                if (payload == null)
                {
                    return;
                }
                handler(payload);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
            }
        });

        
    }
}
