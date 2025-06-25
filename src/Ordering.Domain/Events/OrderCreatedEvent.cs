using Ordering.Domain.Abstractions;
using Ordering.Domain.Entities;

namespace Ordering.Domain.Events;

public record OrderCreatedEvent(Order order) : IDomainEvent;
