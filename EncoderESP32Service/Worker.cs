using Shared.Data;
using Shared.Streaming;
using System.Device.Gpio;
using System.IO.Ports;

namespace EncoderESP32Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IStreamPublisher _streamPublisher;
        private readonly SerialPort serial;
        private readonly GpioController gpio;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IStreamPublisher streamPublisher)
        {
            _logger = logger;
            _configuration = configuration;
            _streamPublisher = streamPublisher;
            var port = _configuration.GetValue<string>("EncodersESP32:Port");
            var speed = _configuration.GetValue<int>("EncodersESP32:Baud");
            var pinreset = _configuration.GetValue<int>("EncodersESP32:PinReset");

            serial = new SerialPort() { WriteTimeout = 500, ReadTimeout = 500, PortName = port, BaudRate = speed };
            serial.Open();
            serial.DiscardInBuffer();
            gpio = new GpioController();
            gpio.OpenPin(pinreset, PinMode.Output);
            gpio.Write(pinreset, PinValue.High);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                string line = serial.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    string[] indata = line.Split(',');
                    for(int i = 0; i < indata.Length; i++)
                    {
                        if (int.TryParse(indata[i], out int value))
                        {
                            await _streamPublisher.PublishAsync($"TAG.Encoder{i+1}", new TagMessage() { TagName = $"Encoder{i+1}", TagValue = value });
                        }
                    }
                    serial.DiscardInBuffer();
                    serial.WriteLine("OK");
                }
            }
        }
    }
}