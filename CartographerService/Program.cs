using CartographerService;
using Shared.Messaging;
using Shared.Redis;
using Shared.Redis.Messaging;
using Shared.Redis.Streaming;
using Shared.Serialization;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {            
        services.AddRedisStreaming()
              .AddRedis(hostContext.Configuration)
              .AddSerialization()
              .AddMessaging()
              .AddRedisMessaging()
              .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
