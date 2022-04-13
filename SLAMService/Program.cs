//using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Redis;
using Shared.Redis.Streaming;
using Shared.Serialization;
using Shared.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SLAMService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddStreaming();
                    services.AddRedisStreaming();
                    services.AddRedis(hostContext.Configuration);
                    services.AddSerialization();
                    services.AddHostedService<Worker>();
                  });
    }
}
