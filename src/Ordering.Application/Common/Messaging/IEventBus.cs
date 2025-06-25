namespace Ordering.Application.Common.Messaging;

public interface IEventBus
{
    Task PublishAsync<T>(T message, string queueName);
    void SubscribeAsync<T, TH>(string queueName)
        where T : class
        where TH : IIntegrationEventHandler<T>;
}

public interface IIntegrationEventHandler<in TIntegrationEvent>
{
    Task Handle(TIntegrationEvent @event);
}