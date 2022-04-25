using Shared.Command;
using Shared.Command.Movement;
using Shared.Data;
using Shared.Streaming;

namespace Tester
{
    internal class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStreamSubscriber _streamSubscriber;
        private readonly ICommandHandler<MotorCommand> _commandHandler;

        public Worker(ILogger<Worker> logger, IStreamSubscriber streamSubscriber, 
            ICommandHandler<MotorCommand> commandHandler)
        {
            _logger = logger;
            _streamSubscriber = streamSubscriber;
            _commandHandler = commandHandler;   
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _streamSubscriber.SubscribeAsync<GamePadMessage>("gamepad", (data) =>
            {
                _logger.LogInformation($"{DateTime.Now} Receiving gamepad: {data.Axes.Count()} axis, {data.Buttons.Count()} buttons");
            });

            //await _streamSubscriber.SubscribeAsync<string>("__keyspace@0__:*", (data) =>
            //{
            //    _logger.LogInformation($"{DateTime.Now} Receiving event: {data} ");
            //});

            //await _streamSubscriber.SubscribeAsync<AlarmMessage>("ALARM.*", (data) => {
            //    _logger.LogInformation($"{DateTime.Now} Receiving alarm: {data.Name} {data.Active}");
            //});
            
            await _streamSubscriber.SubscribeAsync<TagMessage>("TAG.*", (data) =>
            {
                _logger.LogInformation($"{DateTime.Now} Receiving alarm: {data.TagName} {data.TagValue}");
            });
            
            //await _streamSubscriber.SubscribeAsync<ObjectDetectionFrameMessage>("object_detection", (data) =>
            //{
            //    _logger.LogInformation($"{DateTime.Now} Receiving frame: {data.Detections.Count()}");
            //    foreach(var detection in data.Detections)
            //    {
            //        _logger.LogInformation($"{detection.Name} {detection.Confidence} {detection.Left} {detection.Right} {detection.Top} {detection.Bottom}");
            //    }
            //});


            int i = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"send {i}");
                _ = _commandHandler.HandleAsync(new MotorCommand() { MovementType=MovementType.Speed,
                    Acceleration=1000, Motor1Speed=1000, Motor2Speed=1000 });
                i++;
                await Task.Delay(20000);
            }
        }
    }
}