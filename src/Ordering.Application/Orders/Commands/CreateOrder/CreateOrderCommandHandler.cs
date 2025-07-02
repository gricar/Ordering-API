using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Common.Messaging;
using Ordering.Application.Common.Messaging.Events;
using Ordering.Application.Data;
using Ordering.Application.Orders.DTOs;
using Ordering.Domain.Entities;
using Ordering.Domain.ValueObjects;

namespace Ordering.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IApplicationDbContext dbContext,
    IEventBus eventBus,
    ILogger<CreateOrderCommandHandler> logger)
    : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var newOrder = Order.Create(
            OrderId.Of(Guid.NewGuid()),
            CustomerId.Of(command.CustomerId)
        );

        foreach (var item in command.OrderItems)
        {
            newOrder.Add(
                ProductId.Of(item.ProductId),
                item.Quantity,
                item.Price
            );
        }

        dbContext.Orders.Add(newOrder);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Order saved in database: {id}", newOrder.Id);

        var orderDto = new OrderDto(
            newOrder.Id.Value,
            newOrder.CustomerId.Value,
            newOrder.OrderItems.Select(x => new OrderItemDto(newOrder.Id.Value, x.ProductId.Value, x.Quantity, x.Price)).ToList(),
            newOrder.TotalPrice);

        var orderEvent = new OrderCreatedEvent(orderDto);

        await eventBus.PublishAsync(orderEvent, "order-created");

        return newOrder.Id.Value;
    }
}
