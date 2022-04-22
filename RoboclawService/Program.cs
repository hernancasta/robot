using RoboclawService;
using Shared.Command;
using Shared.Command.Movement;
using Shared.Messaging;
using Shared.Redis;
using Shared.Redis.Messaging;
using Shared.Redis.Streaming;
using Shared.Serialization;
using Shared.Streaming;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        //        services.AddSingleton<ICommandHandler<MotorCommand>, CommandHandler<MotorCommand>>();
        //        services.AddHostedService<Worker>();

        services.AddStreaming()
            .AddRedisStreaming()
            .AddRedis(hostContext.Configuration)
            .AddSerialization()
            .AddMessaging()
            .AddRedisMessaging()
            .AddSingleton<RoboclawService.Roboclaw.Roboclaw>()
            .AddHostedService<MotorCommandListener>() //listen messages of type MotorCommand.
            ;
    })
    .Build();

await host.RunAsync();
