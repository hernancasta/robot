using Shared.Data;
using Shared.Streaming;
using System.Collections.Concurrent;

namespace CartographerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStreamSubscriber _streamSubscriber;

        public Worker(ILogger<Worker> logger, IStreamSubscriber streamSubscriber )
        {
            _logger = logger;
            _streamSubscriber = streamSubscriber;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LidarMessage lastScan = null;

            ConcurrentQueue<Scan> scans = new ConcurrentQueue<Scan>();
            
            await _streamSubscriber.SubscribeAsync<LidarMessage>("lidar", (data) => {
                lastScan = data;
            });

            await _streamSubscriber.SubscribeAsync<TagGroupMessage>("TAGGROUP.Encoders", (tag) =>
            {

                if (lastScan == null) return;

                double CurrentEncoder1 = double.MinValue, CurrentEncoder2 = double.MinValue;

                foreach (var t in tag.Tags)
                {
                    switch (t.TagName)
                    {
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

                scans.Enqueue(new Scan()
                {
                    Encoder1 = (Int32)CurrentEncoder1,
                    Encoder2 = (Int32)CurrentEncoder2,
                    Measurements = lastScan.Measurements.Where(x => x.Quality > 8 && x.Distance > 0).ToList()
                });

            });

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time} {count}", DateTimeOffset.Now, scans.Count);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}