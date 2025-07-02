using MediatR;
using Ordering.Domain.Enums;

namespace Ordering.Application.Orders.Commands.UpdateOrder;

public sealed record UpdateOrderStatusCommand(Guid Id, OrderStatus Status) : IRequest<Unit>;
