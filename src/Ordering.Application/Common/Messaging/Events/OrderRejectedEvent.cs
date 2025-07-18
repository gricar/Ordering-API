namespace Ordering.Application.Common.Messaging.Events;

public record OrderRejectedEvent(Guid OrderId) : IntegrationEvent;
