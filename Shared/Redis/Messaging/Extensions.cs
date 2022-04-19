using Microsoft.Extensions.DependencyInjection;
using Shared.Messaging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Redis.Messaging
{
    internal static class Extensions
    {
        public static IServiceCollection AddRedisMessaging(this IServiceCollection services) =>
            services
            .AddSingleton<IMessagePublisher, RedisMessagePublisher>()
            .AddSingleton<IMessageSubscriber, RedisMessageSubscriber>();

        internal static async Task Queue(this IConnectionMultiplexer connection, RedisKey stackName, RedisValue value)
        {
            await connection.GetDatabase().ListRightPushAsync(stackName, value);
        }

        internal static async Task<RedisValue> Dequeue(this IConnectionMultiplexer connection, RedisKey stackName)
        {
            return await connection.GetDatabase().ListLeftPopAsync(stackName);
        }

    }

}
