using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Command.Movement
{
    internal class SetEncoderCommand : Command
    {
        public UInt32 Count1 { get; set; }
        public UInt32 Count2 { get; set; }

        public override string TopicName => "SetEncoderCommand";
    }
}
