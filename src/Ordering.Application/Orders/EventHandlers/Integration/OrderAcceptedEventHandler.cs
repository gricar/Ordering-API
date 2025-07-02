using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Common.Messaging;
using Ordering.Application.Common.Messaging.Events;
using Ordering.Application.Orders.Commands.UpdateOrder;
using Ordering.Domain.Enums;

namespace Ordering.Application.Orders.EventHandlers.Integration;

public class OrderAcceptedEventHandler(
    ISender sender, ILogger<OrderAcceptedEventHandler> logger)
    : IIntegrationEventHandler<OrderAcceptedEvent>
{
    public async Task Handle(OrderAcceptedEvent @event)
    {
        logger.LogInformation("Integration Event handled: {IntegrationEvent}", @event.EventType);

        logger.LogInformation("Order {OrderId} was accepted by Kitchen MS.", @event.Order.Id);

        await sender.Send(new UpdateOrderStatusCommand(@event.Order.Id, OrderStatus.Preparing));

        await Task.CompletedTask;
    }
}
