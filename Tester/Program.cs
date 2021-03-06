using Shared.Messaging;
using Shared.Redis;
using Shared.Redis.Messaging;
using Shared.Redis.Streaming;
using Shared.Serialization;
using Shared.Streaming;
using Shared.Command;
using Tester;
using Shared.Command.Movement;

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
            .AddSingleton<ICommandHandler<MotorCommand>, CommandHandler<MotorCommand>>(); //Service to send commands to motor drive.
    })
    .Build();

await host.RunAsync();
