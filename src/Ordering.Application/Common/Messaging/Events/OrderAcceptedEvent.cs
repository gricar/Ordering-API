using Ordering.Application.Orders.DTOs;

namespace Ordering.Application.Common.Messaging.Events;

public record OrderAcceptedEvent(OrderDto Order) : IntegrationEvent;
