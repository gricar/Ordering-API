using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Common.Messaging;
using System.Reflection;

namespace Ordering.Application.Extensions;

public static class IntegrationEventHandlerExtensions
{
    public static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>)))
            .ToList()
            .ForEach(handlerType => services.AddTransient(handlerType));

        return services;
    }
}
