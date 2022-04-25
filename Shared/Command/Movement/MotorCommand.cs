using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Command.Movement
{
    internal class MotorCommand : Command
    {

    }

    internal class SpeedMotorCommand : MotorCommand
    {
        public int Motor1Speed { get; set; }

        public int Motor2Speed { get; set; }

        public uint Acceleration { get; set; }

    }

    internal class PositionMotorCommand : SpeedMotorCommand
    {
        public uint Position1 { get; set; }
        public uint Position2 { get; set; }

    }
}
