using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Data;
using Ordering.Application.Exceptions;
using Ordering.Domain.ValueObjects;

namespace Ordering.Application.Orders.Commands.UpdateOrder;

public sealed class UpdateOrderCommandHandler(
    IApplicationDbContext dbContext, ILogger<UpdateOrderCommandHandler> logger)
    : IRequestHandler<UpdateOrderStatusCommand, Unit>
{
    public async Task<Unit> Handle(UpdateOrderStatusCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating order {OrderId} status to {Status}.", command.OrderId, command.Status);

        var orderId = OrderId.Of(command.OrderId);

        var order = await dbContext.Orders.FindAsync(orderId, cancellationToken);
        //var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id.Value == command.OrderId, cancellationToken);

        logger.LogInformation("Found order {OrderId} with status {Status}.", order?.Id, order?.Status);

        if (order is null)
        {
            throw new OrderNotFoundException(command.OrderId);
        }

        order.Update(command.Status);
        //atualizar apenas status no DB --> verificar
        dbContext.Orders.Update(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {OrderId} status updated to {Status}.", command.OrderId, command.Status);

        return Unit.Value;
    }
}
