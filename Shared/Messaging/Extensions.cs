using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Messaging
{
    internal static class Extensions
    {
        internal static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            services.AddSingleton<IMessagePublisher, DefaultMessagePublisher>();
            services.AddSingleton<IMessageSubscriber, DefaultMessageSubscriber>();
            return services;
        }
    }
}
