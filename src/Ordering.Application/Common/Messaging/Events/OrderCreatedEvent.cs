using Ordering.Application.Orders.DTOs;

namespace Ordering.Application.Common.Messaging.Events;

public record OrderCreatedEvent(OrderDto Order) : IntegrationEvent;
