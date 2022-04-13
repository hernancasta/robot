using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Streaming
{
    internal static class Extensions
    {
        internal static IServiceCollection AddStreaming(this IServiceCollection services)
        {
            services.AddSingleton<IStreamPublisher, DefaultStreamPublisher>();
            services.AddSingleton<IStreamSubscriber, DefaultStreamSubscriber>();
            return services;
        }
    }
}
