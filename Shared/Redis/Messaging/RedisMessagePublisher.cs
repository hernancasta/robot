using Shared.Messaging;
using Shared.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Redis.Messaging
{
    internal sealed class RedisMessagePublisher : IMessagePublisher
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ISerializer _serializer;
        public RedisMessagePublisher(IConnectionMultiplexer connectionMultiplexer, ISerializer serializer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _serializer = serializer;
        }

        public Task PublishAsync<T>(string topic, T message) where T : class, IMessage 
        {
            var payload = _serializer.SerializeBytes(message);
            return _connectionMultiplexer.Queue(topic, payload); 

        }
    }
}
