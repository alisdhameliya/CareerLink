using Dapper;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Models;
using Repositories.Models.Repositories.Models;
using Repositories.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly NpgsqlConnection _dbConnection;

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        NpgsqlConnection dbConnection)
    {
        _hubContext = hubContext;
        _dbConnection = dbConnection;
    }

    // Broadcast to all users
    public async Task SendNotificationAsync(string message, string type)
    {
        // Store in database
        await _dbConnection.ExecuteAsync(
            "INSERT INTO notifications (message, type) VALUES (@Message, @Type)",
            new { Message = message, Type = type });

        // Broadcast to all clients
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
        {
            Message = message,
            Type = type,
            Timestamp = DateTime.UtcNow
        });
    }

    // Send to specific user
    // public async Task SendUserNotificationAsync(int userId, string message, string type)
    // {
    //     // Store in database with user association
    //     await _dbConnection.ExecuteAsync(
    //         "INSERT INTO notifications (user_id, message, type, is_read) VALUES (@UserId, @Message, @Type, false)",
    //         new { UserId = userId, Message = message, Type = type });

    //     // Send to specific user via SignalR
    //     await _hubContext.Clients.Group($"user-{userId}")
    //         .SendAsync("ReceiveNotification", new
    //         {
    //             Message = message,
    //             Type = type,
    //             Timestamp = DateTime.UtcNow
    //         });
    // }

    // Get user's notifications
    public async Task<List<Notification>> GetUserNotificationsAsync()
    {
        return (await _dbConnection.QueryAsync<Notification>(
    "SELECT notification_id AS NotificationId, user_id AS UserId, message, type, is_read AS IsRead, created_at AS CreatedAt FROM notifications ORDER BY created_at DESC"
    )).ToList();
    }

    // Get unread count for user
    public async Task<int> GetUnreadCountAsync()
    {
        return await _dbConnection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM notifications WHERE is_read = false");
    }

    // Mark notification as read
    public async Task MarkAsReadAsync(int notificationId)
    {
        await _dbConnection.ExecuteAsync(
            "UPDATE notifications SET is_read = true WHERE notification_id = @NotificationId",
            new { NotificationId = notificationId });
    }
}