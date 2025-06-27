using Ordering.API.Exceptions;
using Ordering.Application.Common.Messaging;
using Ordering.Application.Common.Messaging.Events;
using Ordering.Application.Orders.EventHandlers.Integration;

namespace Ordering.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        var eventBus = app.Services.GetRequiredService<IEventBus>();

        eventBus.SubscribeAsync<OrderAcceptedEvent, OrderAcceptedEventHandler>("order-accepted");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering API");
                c.RoutePrefix = string.Empty; // Redireciona a url / para o Swagger
            });
        }

        app.UseExceptionHandler(options => { });

        app.MapControllers();

        return app;
    }
}
