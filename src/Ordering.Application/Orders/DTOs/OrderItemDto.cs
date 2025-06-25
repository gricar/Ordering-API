namespace Ordering.Application.Orders.DTOs;

public sealed record OrderItemDto(Guid ProductId, int Quantity, decimal Price);
