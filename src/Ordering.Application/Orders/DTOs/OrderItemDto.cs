namespace Ordering.Application.Orders.DTOs;

public sealed record OrderDto(
    Guid Id,
    Guid CustomerId,
    List<OrderItemDto> OrderItems,
    decimal TotalPrice);


public sealed record OrderItemDto(
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    decimal Price);
