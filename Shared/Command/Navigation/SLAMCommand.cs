using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Command.Navigation
{
    internal class SLAMCommand : Command
    {
        public SLAMCommandType CommandType { get; set; }

        public double[] Parameters { get; set; }

        public override string TopicName => "SLAMCommand";
    }

    internal enum SLAMCommandType
    {
        SetPose,
        Reset
    }
}
