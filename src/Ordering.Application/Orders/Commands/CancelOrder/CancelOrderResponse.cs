namespace Ordering.Application.Orders.Commands.CancelOrder;

public sealed record CancelOrderResponse(
        Guid OrderId,
        bool IsSuccess,
        string Message);
