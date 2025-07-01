// Services/NotificationProducer.cs
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Repositories.Models;
using Repositories.Models.Repositories.Models;

public class NotificationProducer : IDisposable
{
    private readonly IModel _channel;
    private const string ExchangeName = "notifications.exchange";

    public NotificationProducer(IModel channel)
    {
        _channel = channel;
        _channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Fanout,
            durable: true);
    }

    public void PublishNotification(NotificationMessage message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: "",
            basicProperties: null,
            body: body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}