using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Common.Messaging;
using Ordering.Domain.Events;

namespace Ordering.Application.Orders.EventHandlers.Domain;

public class OrderCreatedEventHandler(
    IEventBus eventBus, ILogger<OrderCreatedEventHandler> logger)
    : INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        // unused - implement interceptor
        logger.LogInformation("Domain Event handled: {DomainEvent}", domainEvent.GetType().Name);

        await eventBus.PublishAsync(domainEvent, "order-created");
    }
}
