using Shared.Data;
using Shared.Serialization;
using Shared.Streaming;
using System.Collections.Concurrent;

namespace CartographerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStreamSubscriber _streamSubscriber;
        private readonly ISerializer _serializer;

        public Worker(ILogger<Worker> logger, IStreamSubscriber streamSubscriber, ISerializer serializer )
        {
            _logger = logger;
            _streamSubscriber = streamSubscriber;
            _serializer = serializer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LidarMessage lastScan = null;

            ConcurrentQueue<Scan> scans = new ConcurrentQueue<Scan>();
            
            await _streamSubscriber.SubscribeAsync<LidarMessage>("lidar", (data) => {
                lastScan = data;
            });

            DateTime lastScanTime = DateTime.Now;
            double LastEncoder1 = double.NaN, LastEncoder2 = double.NaN;

            bool isMoving = false;

            bool cancelRequested = false;

            await _streamSubscriber.SubscribeAsync<TagGroupMessage>("TAGGROUP.Encoders", (tag) =>
            {

                if (lastScan == null) return;

                double CurrentEncoder1 = double.NaN, CurrentEncoder2 = double.NaN;

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

                if (!double.IsNaN(LastEncoder1) && !double.IsNaN(LastEncoder2)
                        && !double.IsNaN(CurrentEncoder1) && !double.IsNaN(CurrentEncoder2))
                {
                    if (isMoving) {
                        if ((DateTime.Now-lastScanTime).TotalMilliseconds>100 ||
                            Math.Abs(LastEncoder1 - CurrentEncoder1)>100 ||
                            Math.Abs(LastEncoder2 - CurrentEncoder2) > 100
                        )
                        scans.Enqueue(new Scan()
                        {
                            Encoder1 = (Int32)CurrentEncoder1,
                            Encoder2 = (Int32)CurrentEncoder2,
                            Measurements = lastScan.Measurements.Where(x => x.Quality > 8 && x.Distance > 0).ToList()
                        });
                    } else
                    {
                        if (!double.IsNaN(LastEncoder1) && LastEncoder1!=CurrentEncoder1)
                        {
                            isMoving = true;
                        }
                        if (!double.IsNaN(LastEncoder2) && LastEncoder2 != CurrentEncoder2)
                        {
                            isMoving = true;
                        }
                    }

                    lastScanTime = DateTime.Now;
                    LastEncoder1 = CurrentEncoder1;
                    LastEncoder2 = CurrentEncoder2;
                } else
                {
                    LastEncoder1 = CurrentEncoder1;
                    LastEncoder2 = CurrentEncoder2;
                }

            });

            await _streamSubscriber.SubscribeAsync<GamePadMessage>("gamepad", (data) =>
            {
                if (data.IsPressed(Button.X))
                {
                    cancelRequested = true;
                }
            });

            while (!stoppingToken.IsCancellationRequested && !cancelRequested)
            {
                _logger.LogInformation("Worker running at: {time} {count}", DateTimeOffset.Now, scans.Count);
                await Task.Delay(1000, stoppingToken);
            }

            // write to file.
            var data = _serializer.SerializeBytes(new Scans { scans = scans.ToArray() });

            using (FileStream fs = new FileStream(@"map.data", FileMode.Create))
            {

                fs.Write(data);
            }

            _logger.LogInformation($"Saved map.data file {data.Length} bytes");
        }
    }
}