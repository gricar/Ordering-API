using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Common.Messaging;
using Ordering.Application.Common.Messaging.Events;
using Ordering.Application.Orders.Commands.UpdateOrder;
using Ordering.Domain.Enums;

namespace Ordering.Application.Orders.EventHandlers.Integration;

public class OrderRejectedEventHandler(
    ISender sender, ILogger<OrderRejectedEventHandler> logger)
    : IIntegrationEventHandler<OrderRejectedEvent>
{
    public async Task Handle(OrderRejectedEvent @event)
    {
        logger.LogInformation("Integration Event handled: {IntegrationEvent}", @event.EventType);

        logger.LogInformation("Order {OrderId} was rejected by Kitchen MS.", @event.OrderId);

        await sender.Send(new UpdateOrderStatusCommand(@event.OrderId, OrderStatus.Rejected));

        await Task.CompletedTask;
    }
}
