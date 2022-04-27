using MappingService.SLAM;
using Shared.Data;
using Shared.Streaming;
using System.Numerics;

namespace MappingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStreamSubscriber _streamSubscriber;
        private readonly IConfiguration _configuration;
        private readonly IStreamPublisher _streamPublisher;
        private readonly SlamProcessor slam;

        private Vector3 OdoPose; 

        public Worker(ILogger<Worker> logger, IStreamSubscriber streamSubscriber, 
            IConfiguration configuration, IStreamPublisher streamPublisher, SlamProcessor slamProcessor)
        {
            _logger = logger;
            _streamSubscriber = streamSubscriber;
            _configuration = configuration;
            _streamPublisher = streamPublisher;
            slam = slamProcessor;
        }

        private double LastEncoder1 = double.NaN;
        private double LastEncoder2 = double.NaN;
        private double CurrentEncoder1 = double.NaN;
        private double CurrentEncoder2 = double.NaN;

        private double _WheelDiameter;
        private double _CountsPerTurn;
        private double _DistanceBetweenWheels;

        private static object _lock = new object();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _WheelDiameter = _configuration.GetValue<double>("MappingService:WheelDiameter");
            _CountsPerTurn = _configuration.GetValue<double>("MappingService:CountsPerTurn");
            _DistanceBetweenWheels = _configuration.GetValue<double>("MappingService:DistanceBetweenWheels");

            OdoPose = slam.Pose;// new Vector3(physicalMapSize/2, physicalMapSize/2, 0f);


            //SLAM.SlamProcessor slam = new SLAM.SlamProcessor(physicalMapSize, holeMapSize,
            //    obstacleMapSize, OdoPose, sigmaXY, sigmaTheta, iterations, 0).
            //{
            //    HoleWidth = 0.5f// 1.0f
            //};

            LidarMessage lastScan = null;

            await _streamSubscriber.SubscribeAsync<LidarMessage>("lidar", (data) => {
                lastScan = data;
            });

            System.Collections.Concurrent.ConcurrentQueue<ScanSegment> scans = new System.Collections.Concurrent.ConcurrentQueue<ScanSegment>();

            await _streamSubscriber.SubscribeAsync<TagGroupMessage>("TAGGROUP.Encoders", (tag) => {

                if (lastScan == null) return;

                foreach (var t in tag.Tags) {
                    switch (t.TagName) {
                        case "Encoder1":
                            {
                                CurrentEncoder1 = t.TagValue;
                                break;
                            }
                        case "Encoder2":
                            {
                                CurrentEncoder2 = t.TagValue;
                                break;
                            }
                    }
                }

                // calculo distancia orientacion.

                bool shouldUpdate = false;
                if (!double.IsNaN(LastEncoder1) && !double.IsNaN(LastEncoder2)
                        && !double.IsNaN(CurrentEncoder1) && !double.IsNaN(CurrentEncoder2))
                {
                    var C1 = CurrentEncoder1 - LastEncoder1;
                    var C2 = CurrentEncoder2 - LastEncoder2;
                    // distancia recorrida de una rueda = (wheel count / count_rev) * wheel diam * PI
                    var D1 = (C1 / _CountsPerTurn) * _WheelDiameter * Math.PI;
                    var D2 = (C2 / _CountsPerTurn) * _WheelDiameter * Math.PI;
                    var D = (D1 + D2) / 2;

                    if (Math.Abs(C1) > 100 || Math.Abs(C2) > 100) // min 5 degrees (20 counts per degree)
                    {

                        // angulo rotacion sobre una rueda = PI * dist_entre_ruedas / ( distancia recorrida de una rueda )
                        var Aturns = (D1 - D2) / (2 * _DistanceBetweenWheels * Math.PI); // turns => 1 = 360 degres
                        var A = Aturns * 2 * Math.PI; // RAD

                        var Orientation = (double)OdoPose.Z;
                        Orientation += A;
                        if (Orientation >= Math.PI) Orientation -= 2*Math.PI;
                        if (Orientation < -Math.PI) Orientation += 2*Math.PI;

                        var Y = (double)OdoPose.Y;
                        var X = (double)OdoPose.X;
                        Y += Math.Sin(Orientation) * D;
                        X += Math.Cos(Orientation) * D;

                        OdoPose = new Vector3((float)X, (float)Y, (float)Orientation);

                        scans.Enqueue(new ScanSegment() { 
                            Pose = OdoPose,
                            Rays = lastScan.Measurements.Where(x => x.Quality>8 && x.Distance>0).ToList()
                        });

                        shouldUpdate = true;
                    }
                } else
                {
                    shouldUpdate = true;
                }

                if (shouldUpdate) { 
                    LastEncoder1 = CurrentEncoder1;
                    LastEncoder2 = CurrentEncoder2;
                }

            });


            Queue<ScanSegment> last10 = new Queue<ScanSegment>();
            bool calculationFlag = false;
            while (!stoppingToken.IsCancellationRequested)
            {
                if (scans.Count > 0)
                {
                    if (scans.TryDequeue(out ScanSegment data))
                    {
                        last10.Enqueue(data);
                        if (last10.Count > 10) { last10.Dequeue(); }
                        slam.Update(last10.ToList());
/*
                        Console.WriteLine($" {CurrentEncoder1} {CurrentEncoder2} " +
                            $"{OdoPose.Y.ToString("0.000")} {OdoPose.X.ToString("0.000")} {OdoPose.Z.RadToDeg().ToString("0.000")}" +
                            $" {slam.Pose.Y.ToString("0.000")} {slam.Pose.X.ToString("0.000")} {slam.Pose.Z.RadToDeg().ToString("0.000")}"
                            );*/
                        calculationFlag = true;
                        await _streamPublisher.PublishAsync<PoseMessage>("SLAM.POSE",new PoseMessage() { X = slam.Pose.X, Y=slam.Pose.Y, Orientation = slam.Pose.Z.RadToDeg() });
                    }
                    // OdoPose = slam.Pose;
                } else
                {
                    if (calculationFlag)
                    {
                        OdoPose = slam.Pose;
                        calculationFlag = false;
                    }
                    await Task.Delay(100, stoppingToken);
                }

            }
        }
    }
}