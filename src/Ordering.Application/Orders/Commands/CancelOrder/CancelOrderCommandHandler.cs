using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Application.Data;
using Ordering.Application.Exceptions;
using Ordering.Domain.ValueObjects;

namespace Ordering.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler(
    ILogger<CancelOrderCommandHandler> logger,
    IApplicationDbContext dbContext)
    : IRequestHandler<CancelOrderCommand, CancelOrderResponse>
{
    public async Task<CancelOrderResponse> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var orderId = OrderId.Of(command.OrderId);
        var order = await dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(command.OrderId);
        }

        order.Cancel(command.Justification);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {OrderId} successfully cancelled. New Status: {Status}", order.Id.Value, order.Status);

        return new CancelOrderResponse(order.Id.Value, true, "Order successfully cancelled.");
    }
}
