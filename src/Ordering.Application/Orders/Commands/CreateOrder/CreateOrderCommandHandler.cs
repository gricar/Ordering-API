using MediatR;
using Ordering.Application.Common.Messaging;
using Ordering.Application.Common.Messaging.Events;
using Ordering.Application.Data;
using Ordering.Domain.Entities;
using Ordering.Domain.ValueObjects;

namespace Ordering.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IApplicationDbContext dbContext, IEventBus eventBus) : IRequestHandler<CreateOrderCommand, Guid>
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

        var orderEvent = new OrderCreatedEvent(newOrder);

        await eventBus.PublishAsync(orderEvent, "order-created");

        return newOrder.Id.Value;
    }
}
