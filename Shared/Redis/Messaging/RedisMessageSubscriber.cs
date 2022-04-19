using Shared.Messaging;
using Shared.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Redis.Messaging
{
    internal sealed class RedisMessageSubscriber : IMessageSubscriber
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ISerializer _serializer;
        private readonly ISubscriber _subscriber;
        public RedisMessageSubscriber(IConnectionMultiplexer connectionMultiplexer, ISerializer serializer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _serializer = serializer;
            _subscriber = _connectionMultiplexer.GetSubscriber();
        }

        public async Task SubscribeAsync<T>(string topic, Action<T> handler) where T : class, IMessage 
            => await _subscriber.SubscribeAsync($"__keyspace@0__:{topic}", async (key, data) =>
        {

            if (data == "rpush")
            {
                for(; ;) { 
                    var element = await _connectionMultiplexer.Dequeue(topic);
                    if (element.HasValue)
                    {
                        var payload = _serializer.DeserializeBytes<T>(element);
                        handler(payload);
                    } else
                    {
                        break;
                    }
                }
            }

        });
    }
}
