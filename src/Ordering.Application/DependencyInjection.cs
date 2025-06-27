using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ordering.Application.Behaviors;
using Ordering.Application.Common.Messaging;
using Ordering.Application.Orders.EventHandlers.Integration;
using System.Reflection;

namespace Ordering.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            //config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddSingleton<IEventBus>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<RabbitMQEventBus>>();
            var uri = configuration.GetConnectionString("RabbitMq")!;
            var connectionName = configuration["MessageBroker:ConnectionName"]!;
            return new RabbitMQEventBus(uri, connectionName, logger, sp);
        });

        services.AddTransient<OrderAcceptedEventHandler>();

        return services;
    }
}
