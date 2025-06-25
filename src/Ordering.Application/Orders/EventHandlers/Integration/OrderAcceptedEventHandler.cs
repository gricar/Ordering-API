using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Common.Messaging;
using Ordering.Application.Common.Messaging.Events;

namespace Ordering.Application.Orders.EventHandlers.Integration;

public class OrderAcceptedEventHandler(ISender sender, ILogger<OrderAcceptedEventHandler> logger)
    : IIntegrationEventHandler<OrderAcceptedEvent>
{
    public async Task Handle(OrderAcceptedEvent @event)
    {
        logger.LogInformation("Order {OrderId} was accepted by Kitchen.", @event.order.Id);

        // update order status to Accepted
        //await sender.Send(new UpdateOrderStatusCommand(@event.order.Id, OrderStatus.Processing));

        await Task.CompletedTask;
    }
}
