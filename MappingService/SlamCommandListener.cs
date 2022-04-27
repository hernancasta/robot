using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Command.Navigation;
using Shared.Messaging;

namespace MappingService
{
    internal class SlamCommandListener : Shared.Command.CommandListenerService<SLAMCommand>
    {
        SLAM.SlamProcessor _slam;

        public SlamCommandListener(IMessageSubscriber messageSubscriber,
                            SLAM.SlamProcessor slam
                            ) : base(messageSubscriber)
        {
            _slam = slam;
        }


        protected override void ExecuteCommand(SLAMCommand Message)
        {
            switch (Message.CommandType)
            {
                case SLAMCommandType.SetPose:
                    {
                       
                        break;
                    }
                case SLAMCommandType.Reset:
                    {
                        _slam.Reset();
                        break;
                    }

            }

        }
    }
}
