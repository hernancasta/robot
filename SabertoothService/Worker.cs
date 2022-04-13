using Shared.Data;
using Shared.Streaming;
using System.IO.Ports;

namespace SabertoothService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStreamPublisher _streamPublisher;
        private readonly IConfiguration _configuration;
        private readonly SerialPort serial;
        private static object _lock = new object();
        private bool _alarm = false;

        private bool IsOpen => serial.IsOpen;

        public Worker(ILogger<Worker> logger,IConfiguration configuration, IStreamPublisher streamPublisher)
        {
            _logger = logger;
            _streamPublisher = streamPublisher;
            _configuration = configuration;
            var port = _configuration.GetValue<string>("Sabertooth:Port");
            var speed = _configuration.GetValue<int>("Sabertooth:Baud");

            serial = new SerialPort() { WriteTimeout = 500, ReadTimeout = 500, PortName = port, BaudRate = speed };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            double _Battery = double.MinValue;
            double _Temperature1 = double.MinValue;
            double _Temperature2 = double.MinValue;
            double _Current1 = double.MinValue;
            double _Current2 = double.MinValue;
            double _Speed1 = double.MinValue;
            double _Speed2 = double.MinValue;
            double _SignalA1 = double.MinValue;
            double _SignalA2 = double.MinValue;
            double _SignalS1 = double.MinValue;
            double _SignalS2 = double.MinValue;

            int counterSnapshot = 0;

            bool firstscan = true;
            bool _estop = false;

            var ramp1 = _configuration.GetValue<int>("Sabertooth:Ramp1");
            var ramp2 = _configuration.GetValue<int>("Sabertooth:Ramp2");

            serial.Open();

            MotorSetRamp(1, ramp1);
            MotorSetRamp(2, ramp2);
            SetMotorSpeed(1, 0);
            SetMotorSpeed(2, 0);
            MotorStartup(1);
            MotorStartup(2);


            while (!stoppingToken.IsCancellationRequested && serial.IsOpen)
            {
                var Battery = await SendCommand("M1: GET B");
                var Temperature1 = await SendCommand($"M1: GET T");
                var Temperature2 = await SendCommand($"M2: GET T");
                var Current1 = await SendCommand($"M1: GET C");
                var Current2 = await SendCommand($"M2: GET C");
                var Speed1 = await SendCommand($"M1: GET");
                var Speed2 = await SendCommand($"M2: GET");
                var SignalA1 = await SendCommand($"A1: GET");
                var SignalA2 = await SendCommand($"A2: GET");
                var SignalS1 = await SendCommand($"S1: GET");
                var SignalS2 = await SendCommand($"S2: GET");
                if (_Battery != Battery.value || counterSnapshot==0)
                {
                    _Battery = Battery.value;
                    await _streamPublisher.PublishAsync("TAG.Battery", new TagMessage() { TagName="Battery", TagValue=_Battery });
                }
                if (_Temperature1 != Temperature1.value || counterSnapshot == 0)
                {
                    _Temperature1 = Temperature1.value;
                    await _streamPublisher.PublishAsync("TAG.Temperature1", new TagMessage() { TagName = "Temperature1", TagValue = _Temperature1 });
                }
                if (_Temperature2 != Temperature2.value || counterSnapshot == 0)
                {
                    _Temperature2 = Temperature2.value;
                    await _streamPublisher.PublishAsync("TAG.Temperature2", new TagMessage() { TagName = "Temperature2", TagValue = _Temperature2 });
                }
                if (_Current1 != Current1.value || counterSnapshot == 0)
                {
                    _Current1 = Current1.value;
                    await _streamPublisher.PublishAsync("TAG.Current1", new TagMessage() { TagName = "Current1", TagValue = _Current1 });
                }
                if (_Current2 != Current2.value || counterSnapshot == 0)
                {
                    _Current2 = Current2.value;
                    await _streamPublisher.PublishAsync("TAG.Current2", new TagMessage() { TagName = "Current2", TagValue = _Current2 });
                }
                if (_Speed1 != Speed1.value || counterSnapshot == 0)
                {
                    _Speed1 = Speed1.value;
                    await _streamPublisher.PublishAsync("TAG.Speed1", new TagMessage() { TagName = "Speed1", TagValue = _Speed1 });
                }
                if (_Speed2 != Speed2.value || counterSnapshot == 0)
                {
                    _Speed2 = Speed2.value;
                    await _streamPublisher.PublishAsync("TAG.Speed2", new TagMessage() { TagName = "Speed2", TagValue = _Speed2 });
                }
                if (_SignalA1 != SignalA1.value || counterSnapshot == 0)
                {
                    _SignalA1 = SignalA1.value;
                    await _streamPublisher.PublishAsync("TAG.SignalA1", new TagMessage() { TagName = "SignalA1", TagValue = _SignalA1 });
                }
                if (_SignalA2 != SignalA2.value || counterSnapshot == 0)
                {
                    _SignalA2 = SignalA2.value;
                    await _streamPublisher.PublishAsync("TAG.SignalA2", new TagMessage() { TagName = "SignalA2", TagValue = _SignalA2 });
                }
                if (_SignalS1 != SignalS1.value || counterSnapshot == 0)
                {
                    _SignalS1 = SignalS1.value;
                    await _streamPublisher.PublishAsync("TAG.SignalS1", new TagMessage() { TagName = "SignalS1", TagValue = _SignalS1 });
                }
                if (_SignalS2 != SignalS2.value || counterSnapshot == 0)
                {
                    _SignalS2 = SignalS2.value;
                    await _streamPublisher.PublishAsync("TAG.SignalS2", new TagMessage() { TagName = "SignalS2", TagValue = _SignalS2 });
                }
                var estop = _SignalA1 < 0 && _SignalA2 < 0; 

                //check if EStop is pressed
                if (firstscan || estop!=_estop || counterSnapshot == 0)
                {
                    _estop = estop;
                    await _streamPublisher.PublishAsync("ALARM.EStopPressed", new AlarmMessage { Name = "EStopPressed", Active = _estop });
                    firstscan = false;
                }


                await Task.Delay(100);
                counterSnapshot++;
                if (counterSnapshot == 20) counterSnapshot = 0;
            }
        }

        private void MotorSetRamp(int motor, int value)
        {
            if (IsOpen)
            {
                SendCommandNoFeedback($"R{motor}: {value}");
            }
        }

        private bool SendCommandNoFeedback(string message)
        {
            if (!System.Threading.Monitor.TryEnter(_lock, 1000))
            {
                Console.WriteLine("TimeOut sending command");
                return false;
            }
            bool success = false;
            try
            {
                serial.WriteLine(message);
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                System.Threading.Monitor.Exit(_lock);
            }
            return success;
        }
        private async Task<SabertoothMessage> SendCommand(string message)
        {
            string result = null;

            if (!System.Threading.Monitor.TryEnter(_lock, 1000))
            {
                Console.WriteLine("TimeOut sending command");
                return SabertoothMessage.Error;
            }
            try
            {
                serial.WriteLine(message);
                result = serial.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await Task.Delay(200); // gives times to clear any remaining data;
                serial.DiscardInBuffer();
                return SabertoothMessage.Error;
            }
            finally
            {
                System.Threading.Monitor.Exit(_lock);
            }
            //if (string.IsNullOrEmpty(result))
            //{
            //    return SabertoothMessage.Error;
            //}
            return new SabertoothMessage(result);
        }

        private void SetMotorSpeed(int motornumber, int value)
        {
            if (IsOpen)
            {
                if (!_alarm)
                    SendCommandNoFeedback($"M{motornumber}: {value}");
                else
                    SendCommandNoFeedback($"M{motornumber}: 0");

            }
        }

        private void MotorStartup(int motornumber)
        {
            if (IsOpen)
            {
                SendCommandNoFeedback($"M{motornumber}: STARTUP");
            }
        }

        private async Task CleanBuffer()
        {
            if (!System.Threading.Monitor.TryEnter(_lock, 1000))
            {
                return;
            }
            try
            {
                await Task.Delay(200); // gives times to clear any remaining data;
                serial.DiscardInBuffer();
            }
            finally
            {
                System.Threading.Monitor.Exit(_lock);
            }

        }

    }
}