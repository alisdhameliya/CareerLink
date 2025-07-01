using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IUserContext _userContext;

    public NotificationsController(
        INotificationService notificationService,
        IUserContext userContext)
    {
        _notificationService = notificationService;
        _userContext = userContext;
    }
    // [Authorize]
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] bool unreadOnly = false)
    {
        // var userId = User.FindFirst("uid")?.Value;
        // var userIdInt = userId != null ? Convert.ToInt32(userId) : 0;
        // if (userId == null) return Unauthorized();

        var notifications = await _notificationService.GetUserNotificationsAsync();

        if (unreadOnly)
        {
            notifications = notifications.Where(n => !n.IsRead).ToList();
        }

        return Ok(notifications);
    }
    // [Authorize]
    [Authorize]
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        // var userId = User.FindFirst("uid")?.Value;
        // var userIdInt = userId != null ? Convert.ToInt32(userId) : 0;
        // if (userId == null) return Unauthorized();

        var count = await _notificationService.GetUnreadCountAsync();
        return Ok(new { count });
    }
    // [Authorize]
    [Authorize]
    [HttpPost("mark-as-read/{notificationId}")]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        await _notificationService.MarkAsReadAsync(notificationId);
        return Ok();
    }
}