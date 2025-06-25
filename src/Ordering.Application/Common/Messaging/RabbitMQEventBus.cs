using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Ordering.Application.Common.Messaging;

public class RabbitMQEventBus : IEventBus
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQEventBus> _logger;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQEventBus(
        string uri,
        string connectionName,
        ILogger<RabbitMQEventBus> logger,
        IServiceProvider serviceProvider)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(uri),
            ClientProvidedName = connectionName
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<T>(T message, string queueName)
    {
        _channel.QueueDeclare(queueName, true, false, false, null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        _channel.BasicPublish(string.Empty, queueName, true, null, body);

        _logger.LogInformation("Message published to queue {QueueName} with message: {Message}", queueName, message);

        await Task.CompletedTask;
    }

    public void SubscribeAsync<T, TH>(string queueName)
        where T : class
        where TH : IIntegrationEventHandler<T>
    {
        _logger.LogInformation("Subscribing to queue '{QueueName}' for event '{EventType}' with handler '{HandlerType}'.",
            queueName, typeof(T).Name, typeof(TH).Name);

        _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        // Configura o Quality of Service (QoS)
        // Isso garante que o consumidor não pegue mais de 1 mensagem por vez antes de um ACK,
        // evitando que mensagens sejam perdidas se o consumidor falhar.
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var integrationEvent = JsonSerializer.Deserialize<T>(message);

                if (integrationEvent != null)
                {
                    // Resolve o handler do container de DI
                    // Usando CreateScope para garantir que dependências injetadas no handler
                    // sejam resolvidas para esta requisição/mensagem e descartadas corretamente.
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var handler = scope.ServiceProvider.GetRequiredService<TH>();

                        _logger.LogInformation("Handling event '{EventType}' from queue '{QueueName}'. Message: {Message}",
                            typeof(T).Name, queueName, message);

                        await handler.Handle(integrationEvent);
                    }

                    // Acknowledge a mensagem após o processamento bem-sucedido
                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                    _logger.LogInformation("Message acknowledged for event '{EventType}' from queue '{QueueName}'.", typeof(T).Name, queueName);
                }
                else
                {
                    _logger.LogError("Failed to deserialize message to type {EventType} from queue {QueueName}. Message: {Message}", typeof(T).Name, queueName, message);
                    // Nack a mensagem para que ela seja reprocessada ou movida para DLQ (Dead Letter Queue)
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true); // requeue: true pode causar loop infinito se o erro for na desserialização
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message from queue {QueueName}. Message: {Message}", queueName, message);
                // Nack a mensagem em caso de erro, com re-fila (requeue: true) ou para DLQ (requeue: false)
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true); // Considere 'requeue: false' e configurar uma DLQ para erros persistentes
            }
        };

        // Inicia o consumo da fila
        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

    }
}
