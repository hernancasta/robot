using Microsoft.Extensions.DependencyInjection;
using Shared.Streaming;

namespace Shared.Redis.Streaming
{
    internal static class Extensions
    {
        public static IServiceCollection AddRedisStreaming(this IServiceCollection services) =>
            services
            .AddSingleton<IStreamPublisher, RedisStreamPublisher>()
            .AddSingleton<IStreamSubscriber, RedisStreamSubscriber>();
    }
}
