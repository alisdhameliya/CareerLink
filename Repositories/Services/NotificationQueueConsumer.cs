using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Repositories.Interfaces;
using Repositories.Models;
using Repositories.Models.Repositories.Models;

namespace Repositories.Services
{
    public class NotificationQueueConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        private const string QueueName = "notifications.queue";

        public NotificationQueueConsumer(
            IConnection connection,
            IServiceProvider serviceProvider)
        {
            _connection = connection;
            _channel = _connection.CreateModel();
            _serviceProvider = serviceProvider;

            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            _channel.QueueBind(
                queue: QueueName,
                exchange: "notifications.exchange",
                routingKey: "");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<NotificationMessage>(Encoding.UTF8.GetString(body));

                // Store in database
                var notification = new Notification
                {
                    UserId = message.UserId,
                    Message = message.Content,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    Type = message.Type
                };

                await notificationService.SendNotificationAsync(message.Content, message.Type);
            };

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }
    }
}