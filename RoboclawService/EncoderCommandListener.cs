using Shared.Command.Movement;
using Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboclawService
{
    internal class EncoderCommandListener : Shared.Command.CommandListenerService<SetEncoderCommand>
    {
        private readonly Roboclaw.Roboclaw _client;
        public EncoderCommandListener(IMessageSubscriber messageSubscriber,
                                    Roboclaw.Roboclaw client
                                    ) : base(messageSubscriber)
        {
            _client = client;
        }

        protected override void ExecuteCommand(SetEncoderCommand Message)
        {
            bool commandResult = false;

            commandResult = _client.SetEncoder1(Message.Count1);
            if (!commandResult) Console.WriteLine($"Error setting Encoder 1 value {Message.Count1}");
            commandResult = _client.SetEncoder2(Message.Count2);
            if (!commandResult) Console.WriteLine($"Error setting Encoder 2 value {Message.Count2}");
        }
    }
}
