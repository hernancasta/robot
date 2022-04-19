using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Command
{
    internal interface ICommandHandler<T> where T : Command
    {
        Task HandleAsync(T command);
    }
}
