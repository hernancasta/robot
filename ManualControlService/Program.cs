using ManualControlService;
using Shared.Command;
using Shared.Command.Movement;
using Shared.Command.Preset;
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
//                .AddHostedService<PresetCommandListener>()
                .AddSingleton<ICommandHandler<MotorCommand>, CommandHandler<MotorCommand>>() //Service to send commands to motor drive.
                .AddSingleton<ICommandHandler<PresetCommand>, CommandHandler<PresetCommand>>() //Service to send commands to motor drive.
                .AddHostedService<Worker>()
                ;
    })
    .Build();

await host.RunAsync();
