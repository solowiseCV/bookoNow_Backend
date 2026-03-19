using BookNow.Application.Features.Notifications.Request.Queries;
using BookNow.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BookNow.Presentation.Controllers;

[ApiController]
[Route("notifications")]
[Authorize]
public class NotificationsController(ISender sender) : BaseApiController
{    [SwaggerOperation(Summary = "Retrieves all notifications for the currently authenticated user")]
    [HttpGet("my-notifications")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]

    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BookNow.Application.DTOs.Notification.NotificationResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyNotifications()
    {
        var result = await sender.Send(new GetUserNotificationsQuery(UserId));
        return HandleResult(result);
    }
}
