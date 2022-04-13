using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Shared.Redis
{
    internal static class Extensions
    {
        internal static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration) {
            var section = configuration.GetSection("redis");
            var options = new RedisOptions();
            section.Bind(options);
            services.Configure<RedisOptions>(section);
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options.connectionString));

            return services;
        }
    }
}
