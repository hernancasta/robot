using Shared.Command.Preset;
using Shared.Data;
using Shared.Messaging;
using Shared.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualControlService
{
    internal class PresetCommandListener : Shared.Command.CommandListenerService<PresetCommand>
    {
        private readonly IStreamPublisher _streamPublisher;

        public PresetCommandListener(IMessageSubscriber messageSubscriber, IStreamPublisher streamPublisher)
            : base(messageSubscriber, "PresetCommand.ManualControlService") { 
            _streamPublisher = streamPublisher;

        }

        public uint Acceleration { get; private set; }
        public uint Deceleration { get; private set; }

        protected override async void ExecuteCommand(PresetCommand Message)
        {
            switch (Message.Name)
            {
                case "Acceleration":
                    Acceleration = (uint)Message.Value;
                    break;
                case "Deceleration":
                    Deceleration = (uint)Message.Value;
                    break;
            }
            Console.WriteLine($"Received Preset {Message.Topic} {Message.Name} {Message.Value}");

            await _streamPublisher.PublishAsync("Preset."+Message.TopicName , new PresetMessage() { Category = Message.Topic, Name=Message.Name, CurrentValue = Message.Value, SetValue=Message.Value });
        }


    }
}
