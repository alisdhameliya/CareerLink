using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.Extensions.Configuration;

// using Microsoft.Extensions.Configuration;
using MimeKit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Repositories.Model;
using StackExchange.Redis;

namespace TaskTrackPro.Core.Services.Email
{
    public class UserRegistrationConsumer
    {
        private readonly IConnection _rabbitConnection;
        private readonly IDatabase _redis;
        private readonly IConfiguration _config;
        private readonly IModel _channel;

        public UserRegistrationConsumer(IConnection rabbitConnection, IConnectionMultiplexer redis, IConfiguration config)
        {
            _rabbitConnection = rabbitConnection;
            _redis = redis.GetDatabase();
            _config = config;
            _channel = _rabbitConnection.CreateModel(); // Persistent channel
        }

        public void StartListening()
        {
            _channel.QueueDeclare(queue: "UserRegistrationQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Message received from RabbitMQ: {message}");

                var user = JsonSerializer.Deserialize<t_user>(message);
                if (user != null)
                {
                    Console.WriteLine($"Processing registration for {user.c_email}");
                    await SendEmailToAdmin(user.c_email, user.c_username, user.c_userRole);
                    Console.WriteLine("Email sending process completed.");
                }
            };

            _channel.BasicConsume(queue: "UserRegistrationQueue", autoAck: true, consumer: consumer);
            Console.WriteLine("Listening for new user registrations...");
        }

        private async Task SendEmailToAdmin(string userEmail, string userName, string userRole)
        {
            Console.WriteLine("Email: " + userEmail + " Name: " + userName, " Role: " + userRole);
            var emailSettings = _config.GetSection("EmailSettings");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Career Link", emailSettings["SenderEmail"]));
            email.To.Add(new MailboxAddress("Admin", emailSettings["AdminEmail"]));
            email.Subject = "📩 New User Registration - CareerLink";

            // HTML Email Body
            //changes done here
            string emailBody = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        padding: 20px;
                        display: flex;
                        justify-content: center;
                    }}
                    .container {{
                        max-width: 600px;
                        background: white;
                        padding: 20px;
                        border-radius: 10px;
                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                        text-align: center;
                    }}
                    .logo {{
                        width: 200px; /* Adjust width as needed */
                        height: 50px; /* Maintain aspect ratio */
                        margin-bottom: 5px;
                        object-fit:contain;
                    }}
                    .content {{
                        font-size: 16px;
                        color: #333;
                        line-height: 1.6;
                        padding: 20px 0;
                    }}
                    .button {{
                        display: inline-block;
                        padding: 12px 20px;
                        color: white;
                        background-color:rgb(168, 188, 217);
                        text-decoration: none;
                        border-radius: 5px;
                        font-size: 16px;
                        font-weight: bold;
                        margin-top: 15px;
                        text-decoration: none;
                    }}
                    .footer {{
                        font-size: 14px;
                        color: #777;
                        margin-top: 20px;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <img src=""https://res.cloudinary.com/dhruvil20/image/upload/v1743946773/2removebg-preview_x58xrm.png"" alt=""Company Logo"" class=""logo"">
                    <div class=""content"">
                        <h2>{userName}</h2>
                        <p>Email Id <strong>{userEmail}</strong> has been successfully <b>registered</b> as <strong>{userRole}</strong> on <b>CareerLink</b>.</p>
                        <a href=""../RecruiterDashboard/Index"" class=""button"">Go to Dashboard</a>
                    </div>
                    <div class=""footer"">©CareerLink. All Rights Reserved.</div>
                </div>
            </body>
            </html>";

            email.Body = new TextPart("html") { Text = emailBody };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["SenderPassword"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            Console.WriteLine($"Admin notified via email: {emailSettings["AdminEmail"]}");
        }

    }
}


// <a href='https://yourwebsite.com/admin-dashboard' class='button'>View User</a>