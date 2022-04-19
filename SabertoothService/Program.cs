using SabertoothService;
using Shared.Messaging;
using Shared.Redis;
using Shared.Redis.Messaging;
using Shared.Redis.Streaming;
using Shared.Serialization;
using Shared.Streaming;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddStreaming()
                .AddRedisStreaming()
                .AddRedis(hostContext.Configuration)
                .AddSerialization()
                .AddHostedService<Worker>()
                .AddMessaging()
                .AddRedisMessaging()
                .AddHostedService<MotorCommandListener>();
    })
    .Build();

await host.RunAsync();
