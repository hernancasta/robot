using Shared.Data;
using Shared.Streaming;

namespace Tester
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
            await _streamSubscriber.SubscribeAsync<GamePadMessage>("gamepad", (data) =>
            {
                _logger.LogInformation($"{DateTime.Now} Receiving gamepad: {data.Axes.Count()} axis, {data.Buttons.Count()} buttons");
            });

            //await _streamSubscriber.SubscribeAsync<AlarmMessage>("ALARM.*", (data) => {
            //    _logger.LogInformation($"{DateTime.Now} Receiving alarm: {data.Name} {data.Active}");
            //});

            //await _streamSubscriber.SubscribeAsync<TagMessage>("TAG.*", (data) => {
            //    _logger.LogInformation($"{DateTime.Now} Receiving alarm: {data.TagName} {data.TagValue}");
            //});

            //await _streamSubscriber.SubscribeAsync<ObjectDetectionFrameMessage>("object_detection", (data) =>
            //{
            //    _logger.LogInformation($"{DateTime.Now} Receiving frame: {data.Detections.Count()}");
            //    foreach(var detection in data.Detections)
            //    {
            //        _logger.LogInformation($"{detection.Name} {detection.Confidence} {detection.Left} {detection.Right} {detection.Top} {detection.Bottom}");
            //    }
            //});
        }
    }
}