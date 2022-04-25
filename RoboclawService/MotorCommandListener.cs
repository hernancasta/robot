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

            if (Message is PositionMotorCommand)
            {
                commandResult = _client.MixedSpeedAccelDistance(
                    (Message as PositionMotorCommand).Acceleration,
                    (Message as PositionMotorCommand).Motor1Speed,
                    (Message as PositionMotorCommand).Position1,
                    (Message as PositionMotorCommand).Motor2Speed,
                    (Message as PositionMotorCommand).Position2,1)
                    ;
            } else if (Message is SpeedMotorCommand)
            {
                commandResult = _client.MixedSpeedAccel(
                    (Message as SpeedMotorCommand).Acceleration,
                    (Message as SpeedMotorCommand).Motor1Speed,
                    (Message as SpeedMotorCommand).Motor2Speed
                    );
            } 

/*
            if (commandResult)
                Console.WriteLine("Executed Movement");
            else
                Console.WriteLine("Error receiving data");
  */          
        }
    }
}
