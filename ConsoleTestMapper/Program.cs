using Microsoft.Extensions.Hosting;
using Shared.Command;
using Shared.Command.Movement;
using Shared.Messaging;
using Shared.Redis.Messaging;
using System.IO;
using System.Numerics;
using Shared.Redis;
using Shared.Redis.Streaming;
using Shared.Serialization;
using Shared.Streaming;
using Microsoft.Extensions.DependencyInjection;
using ConsoleTestMapper;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

//MakeMap();

await StartHost();



static async Task StartHost() {

    IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddStreaming()
                .AddRedisStreaming()
                .AddRedis(hostContext.Configuration)
                .AddSerialization()
                .AddMessaging()
                .AddRedisMessaging()
                .AddSingleton<ICommandHandler<MotorCommand>, CommandHandler<MotorCommand>>() //Service to send commands to motor drive.
                .AddHostedService<Worker>()
                ;
    })
    .Build();

    await host.RunAsync();

}


static void MakeMap() {

    byte[] data = File.ReadAllBytes("map.data");

    Console.WriteLine(data.Length);

    var serializer = new Shared.Serialization.SystemTextJsonSerializer();
    var obj = serializer.DeserializeBytes<CartographerService.Scans>(data);

    Vector3 Pose = new Vector3(500, 500, 0); //X,Y, Orientation

    SkiaSharp.SKBitmap bitmap = new SkiaSharp.SKBitmap(1000, 1000);

    bool first = true;
    double Last1 = 0;
    double Last2 = 0;

    double ForwardAccum = 0;
    double TurnAccum = 0;

    foreach (var item in obj.scans)
    {
        if (first)
        {
            first = false;
        }
        else
        {
            var W1 = item.Encoder1 - Last1;
            var W2 = item.Encoder2 - Last2;
            var Forward = (W1 + W2) / 2;
            var Turn = (W1 - W2);
            ForwardAccum += Forward;
            TurnAccum += Turn;
            //        Console.WriteLine($"{ForwardAccum} {TurnAccum}");

            var TurnRad = Turn;
            var ForwardDist = Forward / 7200;

            var Y = ForwardDist * Math.Cos(TurnRad);
            var X = ForwardDist * Math.Sin(TurnRad);

            Pose += new Vector3((float)X, (float)Y, (float)TurnRad);

            Console.WriteLine($"{Pose.X} {Pose.Y} {Pose.Z}");
        }
        Last1 = item.Encoder1;
        Last2 = item.Encoder2;

        //    Console.WriteLine($"{item.Encoder1} {item.Encoder2} {item.Measurements.Count}");
        foreach (var Measure in item.Measurements)
        {

        }
    }


}