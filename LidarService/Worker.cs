using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using RPLidar;
using Microsoft.Extensions.Configuration;
using Shared.Streaming;
using System.Collections.Generic;
using Shared.Data;
using static Shared.Data.LidarMessage;

namespace LidarService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IStreamPublisher _streamPublisher;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IStreamPublisher streamPublisher)
        {
            _logger = logger;
            _configuration = configuration;
            _streamPublisher = streamPublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var port = _configuration.GetValue<string>("Lidar:Port");
            var speed = _configuration.GetValue<ushort>("Lidar:Speed");
            var rplidar = new Lidar(port);
            rplidar.ReceiveTimeout = 1000;
            var result = rplidar.Open();
            if (!result) {
                _logger.LogError("Serial port cannot be opened!!!");
                return;
            }
            await Task.Delay(20);
            rplidar.Reset();
            await Task.Delay(20);
            rplidar.SetMotorSpeed(speed);
            await Task.Delay(20);

            while(!stoppingToken.IsCancellationRequested && !rplidar.StartScan(ScanMode.Legacy))
            {
                await Task.Delay(1000);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var scan = rplidar.GetScan(stoppingToken);
                if (scan != null)
                {
                    List<LidarMeasurement> measurements = new List<LidarMeasurement>();
                    foreach(var measurement in scan.Measurements)
                    {
                        measurements.Add(new LidarMeasurement(measurement.IsNewScan, measurement.Angle, measurement.Distance, measurement.Quality));
                    }
                    await _streamPublisher.PublishAsync("lidar", new LidarMessage { Measurements = measurements });
                }
            }

            rplidar.SetMotorSpeed(0);
            rplidar.Reset();
            rplidar.Close();
        }

    }
}