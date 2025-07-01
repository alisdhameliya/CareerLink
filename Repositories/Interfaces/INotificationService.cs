using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Models;
using Repositories.Models.Repositories.Models;

namespace Repositories.Interfaces
{
    public interface INotificationService
    {
       
        // Existing method
        Task SendNotificationAsync(string message, string type);

        // New methods for NotificationsController
        Task<List<Notification>> GetUserNotificationsAsync();
        Task<int> GetUnreadCountAsync();
        Task MarkAsReadAsync(int notificationId);

        // Optional: Add user-specific notification
        // Task SendUserNotificationAsync(int userId, string message, string type);
    }
}