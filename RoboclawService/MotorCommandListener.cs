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

        private readonly Roboclaw.Roboclaw _client;
        public MotorCommandListener(IMessageSubscriber messageSubscriber,
                                    Roboclaw.Roboclaw client
                                    ) : base(messageSubscriber)
        {
            _client = client;
        }

        protected override void ExecuteCommand(MotorCommand Message)
        {
            //            UInt32 status =0;
            //            var result = client.GetStatus(ref status);

            //double temp = 0;
            //var result = client.GetTemperature(ref temp);

            //            var result = client.MixedSpeedDistance(100000, 10000, 100000, 10000, 1);
            //var result = _client.M1Speed(1000);
            //var result = _client.MixedSpeed(2000, 2000);

            int M1cnt=0, M2cnt=0;

            var r = _client.GetEncoders(ref M1cnt, ref M2cnt);

            Console.WriteLine($"{M1cnt} {M2cnt}");

            var result1 = _client.SetEncoder1(0);
            var result2 = _client.SetEncoder2(0);

            var result = _client.MixedSpeedDistance(3000, 10000, 3000, 10000, 0);
            if (result)

                /* *************************************************
                Console.WriteLine($"Status {status.ToString("X")}");
                if (status != 0)
                {
                    foreach(var alarm in status.ToAlarms())
                    {
                        Console.WriteLine(alarm);
                    }
                }
                */

                Console.WriteLine("Executed Movement");
            else
                Console.WriteLine("Error receiving data");

            
        }
    }
}
