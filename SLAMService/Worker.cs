using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Data;
using Shared.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SLAMService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStreamSubscriber _streamSubscriber;

        public Worker(ILogger<Worker> logger, IStreamSubscriber streamSubscriber)
        {
            _logger = logger;
            _streamSubscriber = streamSubscriber;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _streamSubscriber.SubscribeAsync<LidarMessage>("lidar", (data) => {
                _logger.LogInformation($"{DateTime.Now} Receiving lidar: {data.Measurements.Count()} values");
            });
        }
    }
}
