using Shared.Command;
using Shared.Command.Movement;
using Shared.Command.Preset;
using Shared.Data;
using Shared.Streaming;

namespace ManualControlService
{
    internal class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ICommandHandler<MotorCommand> _motor;
        private readonly IStreamSubscriber _streamSubscriber;
        private readonly IConfiguration _configuration;
        private readonly PresetCommandListener _presetListener;
        private readonly ICommandHandler<PresetCommand> _preset;

        public Worker(ILogger<Worker> logger, IStreamSubscriber streamSubscriber,
            ICommandHandler<MotorCommand> commandHandler, IConfiguration configuration, 
            PresetCommandListener presetListener,
            ICommandHandler<PresetCommand> preset
            )
        {
            _logger = logger;
            _motor = commandHandler;
            _streamSubscriber = streamSubscriber;
            _configuration = configuration;
            _preset = preset;
            _presetListener = presetListener;
        }


        bool Enabled = false;
        bool Alarm = false;

        float Vertical = 0;
        float Horizontal = 0;

        bool ManualModeActivated = false;
        bool lastButtonAPressed = false;

        bool _blockLidar = false;
        short _zeroThreshold;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _blockLidar = _configuration.GetValue<bool>("ManualControlService:BlockByLidar");
            _zeroThreshold = _configuration.GetValue<short>("ManualControlService:ZeroThreshold");

           
            await _streamSubscriber.SubscribeAsync<GamePadMessage>("gamepad", async (data) =>
            {
                Enabled = data.IsPressed(Button.LB) & data.IsPressed(Button.RB);

                if (data.IsPressed(Button.A) && !lastButtonAPressed)
                {
                    ManualModeActivated = !ManualModeActivated;
                }
                lastButtonAPressed = data.IsPressed(Button.A);

                short Y = data.GetAxis(Axis.LEFT_STICK_Y); 
                short X = data.GetAxis(Axis.RIGHT_STICK_X);

                if (Math.Abs(Y) < _zeroThreshold)
                {
                    Y = 0;
                }

                if (Math.Abs(X) < _zeroThreshold)
                {
                    X = 0;
                }

                Vertical = (float)Y / -32767f;
                Horizontal = (float)X / 32767f;

                await NotifyStateChanged();
            });

            await _streamSubscriber.SubscribeAsync<LidarMessage>("lidar", async (data) => {
                var closest = data.Measurements.Where(x => x.Distance>0).OrderBy(x => x.Distance).First();
                Alarm = (closest.Distance < 0.6);
                await NotifyStateChanged();
            });

        }

        bool lastMovedFlag;

        private async Task NotifyStateChanged()
        {
            if (ManualModeActivated) { 
                if (Enabled )
                {
                    var motor1 = (Vertical + Horizontal) / 2;
                    var motor2 = (Vertical - Horizontal) / 2;

                    await _motor.HandleAsync(new MotorCommand()
                    {
                        MovementType = MovementType.Speed,
                        Acceleration = _presetListener.GetPreset<UInt32>("Acceleration").CurrentValue,
                        Motor1Speed = (int)(_presetListener.GetPreset<UInt32>("Speed").CurrentValue * motor1),
                        Motor2Speed = (int)(_presetListener.GetPreset<UInt32>("Speed").CurrentValue * motor2)
                    });
                    lastMovedFlag = true;
                } else
                {
                    await _motor.HandleAsync(new MotorCommand()
                    {
                        MovementType = MovementType.Speed,
                        Acceleration = _presetListener.GetPreset<UInt32>("Deceleration").CurrentValue,
                        Motor1Speed = 0,
                        Motor2Speed = 0
                    });
                    lastMovedFlag = false;

                }
            } else
            {
                if (lastMovedFlag)
                {
                    await _motor.HandleAsync(new MotorCommand()
                    {
                        MovementType = MovementType.Speed,
                        Acceleration = 1000,
                        Motor1Speed = 0,
                        Motor2Speed = 0
                    });
                    lastMovedFlag = false;

                }
            }
        }
    }
}