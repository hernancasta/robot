using Shared.Command;
using Shared.Command.Movement;
using Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabertoothService
{
    internal sealed class MotorCommandListener : CommandListenerService<MotorCommand>
    {

        public MotorCommandListener(IMessageSubscriber messageSubscriber):
            base(messageSubscriber, "MotorCommand")
        {
        }

        protected override void ExecuteCommand(MotorCommand Message)
        {
            throw new NotImplementedException();
        }
    }
}
