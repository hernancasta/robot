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

            bool commandResult = false;

            switch (Message.MovementType)
            {
                case MovementType.Speed:
                    {
                        commandResult = _client.MixedSpeedAccel(
                            Message.Acceleration,
                            Message.Motor1Speed,
                            Message.Motor2Speed
                            );
                        break;
                    }
                case MovementType.Position:
                    {
                        commandResult = _client.MixedSpeedAccelDistance(
                            Message.Acceleration,
                            Message.Motor1Speed,
                            Message.Position1,
                            Message.Motor2Speed,
                            Message.Position2, 1)
                            ;
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"Movement type: {Message.MovementType} not defined");
                        break;
                    }
            }


            if (commandResult)
                Console.WriteLine("Executed Movement");
            else
                Console.WriteLine("Error receiving data");

        }
    }
}
