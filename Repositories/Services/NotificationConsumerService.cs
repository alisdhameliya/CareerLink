using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting; // Required for BackgroundService
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Repositories.Services
{
    public class NotificationConsumerService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "job_notifications",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[Notification]: {message}");
            };

            channel.BasicConsume(queue: "job_notifications",
                                 autoAck: true,
                                 consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
