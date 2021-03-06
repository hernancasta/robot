using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Command.Preset
{
    internal class PresetCommand : Command 
    {
        public string Name { get; set; }

        public string Topic { get; set; }

        public object SetValue { get; set; }

        public override string TopicName => $"PresetCommand.{Topic}";
    }
}
