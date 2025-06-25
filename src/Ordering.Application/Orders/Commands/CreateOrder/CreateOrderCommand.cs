using MediatR;
using Ordering.Application.Orders.DTOs;

namespace Ordering.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand : IRequest<Guid>
{
    public Guid CustomerId { get; init; }
    public List<OrderItemDto> OrderItems { get; init; } = new List<OrderItemDto>();
}
