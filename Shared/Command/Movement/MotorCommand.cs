using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Command.Movement
{
    internal class MotorCommand : Command
    {
        public MovementType MovementType { get; set; }



        // For Speed
        public int Motor1Speed { get; set; }

        public int Motor2Speed { get; set; }

        public uint Acceleration { get; set; }

        // For Position
        public uint Position1 { get; set; }
        public uint Position2 { get; set; }

        public override string TopicName => "MotorCommand";
    }

    internal enum MovementType
    {
        Speed,
        Position
    }
}
