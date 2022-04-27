using Shared.Data;
using Shared.Streaming;

namespace GamePadService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IStreamPublisher _streamPublisher;

        private Dictionary<byte, Button> Buttons = new Dictionary<byte, Button>();
        private Dictionary<byte, Axis> Axis = new Dictionary<byte, Axis>();

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IStreamPublisher streamPublisher)
        {
            _logger = logger;
            _configuration = configuration;
            _streamPublisher = streamPublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var deviceFile = _configuration.GetValue<string>("Gamepad:DeviceFile");
            if (!File.Exists(deviceFile))
            {
                throw new ArgumentException(nameof(deviceFile), $"The device {deviceFile} does not exist");
            }
            using (FileStream fs = new FileStream(deviceFile, FileMode.Open))
            {
                byte[] message = new byte[8];

                while (!stoppingToken.IsCancellationRequested)
                {
                    // Read chunks of 8 bytes at a time.
                    await fs.ReadAsync(message, 0, 8);
                    if (message.HasConfiguration())
                    {
                        ProcessConfiguration(message);
                    }

                    await ProcessValuesAsync(message);
                }
            }
        }

        private void ProcessConfiguration(byte[] message)
        {
            if (message.IsButton())
            {
                byte key = message.GetAddress();
                if (!Buttons.ContainsKey(key))
                {
                    Buttons.Add(key, new Button { Index = key, Pressed = false });
                    return;
                }
            }
            else if (message.IsAxis())
            {
                byte key = message.GetAddress();
                if (!Axis.ContainsKey(key))
                {
                    Axis.Add(key, new Axis { Index=key, Value=0});
                    return;
                }
            }
        }

        private async Task ProcessValuesAsync(byte[] message)
        {
            if (message.IsButton())
            {
                var oldValue = Buttons[message.GetAddress()].Pressed;
                var newValue = message.IsButtonPressed();

                if (oldValue != newValue)
                {
                    Buttons[message.GetAddress()].Pressed = message.IsButtonPressed();
                }
            }
            else if (message.IsAxis())
            {
                var oldValue = Axis[message.GetAddress()].Value;
                var newValue = message.GetAxisValue();

                if (oldValue != newValue)
                {
                    Axis[message.GetAddress()].Value = message.GetAxisValue();
                }
            }

            var redismessage = new GamePadMessage {
                Axes = Axis.Values,
                Buttons = Buttons.Values
            };
            await _streamPublisher.PublishAsync("gamepad", redismessage);

        }
    }
}