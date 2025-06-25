using Ordering.Domain.Entities;

namespace Ordering.Application.Common.Messaging.Events;

public record OrderAcceptedEvent(Order order) : IntegrationEvent;
