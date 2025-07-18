﻿using MediatR;

namespace Ordering.Application.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(
        Guid OrderId,
        string Justification) : IRequest<CancelOrderResponse>;


