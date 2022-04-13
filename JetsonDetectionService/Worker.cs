using Shared.Data;
using Shared.Streaming;

namespace JetsonDetectionService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStreamPublisher _streamPublisher;

        public Worker(ILogger<Worker> logger, IStreamPublisher streamPublisher)
        {
            _logger = logger;
            _streamPublisher = streamPublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string s;
            while (!stoppingToken.IsCancellationRequested && (s = Console.ReadLine())!=null)
            {
                if (s.StartsWith("[FRAME]|"))
                {
                    Frame f = new Frame(s);
                    ObjectDetectionFrameMessage payload = new ObjectDetectionFrameMessage()
                    {
                        Detections = from x in f.Detections 
                                     select new ObjectDetectionMessage(
                                                x.ObjectName,
                                                 x.Confidence,
                                                 x.Left,
                                                 x.Right,
                                                 x.Top,
                                                 x.Bottom)
                    };

                    await _streamPublisher.PublishAsync("object_detection", payload);
                }

            }
        }
    }
}