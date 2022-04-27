using MappingService;
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
            .AddMessaging()
            .AddRedisMessaging()
            .AddSingleton<MappingService.SLAM.SlamProcessor>()
            //.AddHostedService<MotorCommandListener>() //listen messages of type MotorCommand.
            .AddHostedService<Worker>()
            ;
    })
    .Build();

await host.RunAsync();
