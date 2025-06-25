using Ordering.Domain.Entities;

namespace Ordering.Application.Common.Messaging.Events;

public record OrderCreatedEvent(Order order) : IntegrationEvent;
