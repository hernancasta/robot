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
using SkiaSharp;
using CartographerService;
using MappingService.SLAM;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

MakeMap();

//await StartHost();

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

    Vector3 Pose = new Vector3(0, 0, 0); //X,Y, Orientation

    SkiaSharp.SKBitmap bitmap = new SkiaSharp.SKBitmap(1000, 1000);

    bool first = true;
    double Last1 = 0;
    double Last2 = 0;

    double ForwardAccum = 0;
    double TurnAccum = 0;

    var counter = 1;

    using (SKCanvas canvas = new SKCanvas(bitmap))
    {
        using (SKPaint paint = new SKPaint())
        {
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = SKColors.White;
            paint.StrokeWidth = 2;
            paint.StrokeCap = SKStrokeCap.Butt;
            paint.TextSize = 30;

            using (SKPaint paint1 = new SKPaint()) { 

                paint1.Color = new SKColor(10,10,10);

                using (SKPaint paint2 = new SKPaint())
                {

                    paint1.Color = new SKColor(20, 20, 20);

                    for (int x = 0; x < 100; x++)
                    {
                        for (int y = 0; y < 100; y++)
                        {
                            var color = (x + y) % 2 == 0 ? paint1 : paint2;
                            canvas.DrawRect(x * 100, y * 100, 100, 100, color);
                        }
                    }

                    var reference = 1000; // 1m => a mm
                    reference /= 10; //scale
                    canvas.DrawLine(new SKPoint(0, 50), new SKPoint(0 + reference, 50), paint);

                    canvas.DrawText("1m", new SKPoint(0, 45), paint);

                    reference = 100; // 10cm => a mm
                    reference /= 10; //scale
                    canvas.DrawLine(new SKPoint(0, 100), new SKPoint(0 + reference, 100), paint);

                    canvas.DrawText("10cm", new SKPoint(0, 95), paint);

                   
                }
            }
        }
    }

    SlamProcessor slam = new SlamProcessor(20f, 1024, 1024, .4f, 2, 1000, 0.5f);

    foreach (var item in obj.scans)
    {

        DrawMeasures(bitmap, item, Pose);

        double ForwardDist = 0;

        if (first)
        {
            first = false;
            // drawing map.
            bitmap.SetPixel(
                (int)(Pose.X / 10 + bitmap.Width / 2),
                bitmap.Height - (int)(Pose.Y / 10 + bitmap.Height / 2),
                new SkiaSharp.SKColor(0, 255, 0)
                );
        }
        else
        {
            var W1 = item.Encoder1 - Last1;
            var W2 = item.Encoder2 - Last2;
            var Forward = (W1 + W2) / 2;
            var Turn = (W1 - W2);
            ForwardAccum += Forward;

            // Distance between wheels = 0.724m
            // D * PI = Distance for 1 turn.
            var TurnRad =(Math.PI) * ((Turn * 930 / 7200) / (Math.PI * 724));
            TurnAccum += TurnRad;
            ForwardDist = 930 * Forward / 7200;
            var Y = ForwardDist * Math.Cos(TurnAccum);
            var X = ForwardDist * Math.Sin(TurnAccum);
            Pose += new Vector3((float)X, (float)Y, (float)TurnRad);

            // drawing map.
            bitmap.SetPixel(
                (int)(Pose.X / 10 + bitmap.Width / 2) ,
                bitmap.Height - (int)(Pose.Y / 10 + bitmap.Height / 2) , 
                new SkiaSharp.SKColor(0, 255, 0)
                );

//            DrawMeasures(bitmap, item, Pose);

        }
        Last1 = item.Encoder1;
        Last2 = item.Encoder2;

        // 1.1327 coef para pegarle a la distancia 
        // 7200 una vuelta = 0.93mts

        ScanSegment scansegment = new ScanSegment() {
            Pose = Pose,
            Rays = item.Measurements
        };
        slam.Update(new ScanSegment[] { scansegment });

        Console.WriteLine($"X={Pose.X}\tY={Pose.Y}\tOrientation={ToGrad(Pose.Z)}\tForward={ForwardDist} " +
            $"XPose={slam.Pose.X}\tYPose={slam.Pose.Y}\tOrientation={ToGrad(slam.Pose.Z)}");


    }
    using (MemoryStream memStream = new MemoryStream())
    using (SKManagedWStream wstream = new SKManagedWStream(memStream))
    {
        bitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
        byte[] data1 = memStream.ToArray();

        using (FileStream fs = new FileStream(@$"map{counter.ToString("0000")}.png", FileMode.Create))
        {

            fs.Write(data1);//, folder, filename);
        }
    }
    counter++;
}

static float ToGrad(float value)
{
    return (float)((value * 180 / Math.PI) % 360);
}

static void DrawMeasures(SkiaSharp.SKBitmap bitmap, Scan item, Vector3 Pose)
{
    var Bounds = ((bitmap.Width - 2) / 2) * 10;
    foreach (var Measure in item.Measurements)
    {
        var px = 1000 * Measure.Distance * Math.Sin(Measure.Angle * Math.PI / 180 + Pose.Z) + Pose.X + 210 * Math.Sin(Pose.Z);
        var py = 1000 * Measure.Distance * Math.Cos(Measure.Angle * Math.PI / 180 + Pose.Z) + Pose.Y + 210 * Math.Cos(Pose.Z);

        if (Math.Abs(px) < Bounds && Math.Abs(py) < Bounds)
        {
            bitmap.SetPixel(
               (int)(px / 10 + bitmap.Width / 2),
               bitmap.Height - (int)(py / 10 + bitmap.Height / 2),
               new SkiaSharp.SKColor(255, 0, 0)
               );
        }
    }
}