using Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Command
{
    internal abstract class Command : IMessage
    {
        abstract public string TopicName { get; }
    }
}
