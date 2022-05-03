using Shared.Command.Movement;
using Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    internal sealed class NotifierMessageBackgroundService : Shared.Command.CommandListenerService<MotorCommand>
    {


        public NotifierMessageBackgroundService(IMessageSubscriber messageSubscriber) : base(messageSubscriber, "MotorCommand")
        {

        }

        protected override void ExecuteCommand(MotorCommand Message)
        {
            throw new NotImplementedException();
        }
    }
}
