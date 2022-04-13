using Shared.Redis;
using Shared.Redis.Streaming;
using Shared.Serialization;
using Shared.Streaming;
using Tester;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddStreaming();
        services.AddRedisStreaming();
        services.AddRedis(hostContext.Configuration);
        services.AddSerialization();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
