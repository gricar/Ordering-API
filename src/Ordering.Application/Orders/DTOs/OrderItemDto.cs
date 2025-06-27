namespace Ordering.Application.Orders.DTOs;

public sealed record OrderDto(
    Guid OrderId,
    Guid CustomerId,
    List<OrderItemDto> OrderItems,
    decimal TotalPrice);


public sealed record OrderItemDto(
    Guid ProductId,
    int Quantity,
    decimal Price);
