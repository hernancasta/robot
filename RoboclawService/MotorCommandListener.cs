using RoboclawService.Roboclaw;
using Shared.Command.Movement;
using Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboclawService
{
    internal class MotorCommandListener : Shared.Command.CommandListenerService<MotorCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly Roboclaw.Roboclaw client;
        public MotorCommandListener(IMessageSubscriber messageSubscriber
                                    ,IConfiguration configuration
                                    ) : base(messageSubscriber)
        {
            _configuration = configuration;
            var port = _configuration.GetValue<string>("Roboclaw:Port");
            var speed = _configuration.GetValue<int>("Roboclaw:Speed");
            var address = _configuration.GetValue<byte>("Roboclaw:Address");

            client = new Roboclaw.Roboclaw(port, speed, address);
            client.Open();
        }

        protected override void ExecuteCommand(MotorCommand Message)
        {
            //            UInt32 status =0;
            //            var result = client.GetStatus(ref status);

            double temp = 0;
            var result = client.GetTemperature(ref temp);
            if (result)
                /*
                Console.WriteLine($"Status {status.ToString("X")}");
                if (status != 0)
                {
                    foreach(var alarm in status.ToAlarms())
                    {
                        Console.WriteLine(alarm);
                    }
                }*/
                Console.WriteLine(temp);
            else
                Console.WriteLine("Error receiving data");
        }
    }
}
