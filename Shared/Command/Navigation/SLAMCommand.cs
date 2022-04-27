using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Command.Navigation
{
    internal class SLAMCommand : Command
    {
        public SLAMCommandType CommandType { get; set; }

        public double[] Parameters { get; set; }
    }

    internal enum SLAMCommandType
    {
        SetPose,
        Reset
    }
}
