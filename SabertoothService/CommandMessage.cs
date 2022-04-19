using Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SabertoothService
{
    internal record CommandMessage(string CommandCode, string Source) : IMessage;

}
