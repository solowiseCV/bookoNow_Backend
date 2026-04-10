using BookNow.Application.Features.Notification.Request.Commands;
using BookNow.Application.Features.Notification.Request.Queries;
using BookNow.Application.Interfaces.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookNow.Presentation.Controllers;

[Route("api/notifications")]
[ApiController]
[Authorize]
public class NotificationController(IMediator mediator, ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var userIdString = currentUserService.UserId;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(new { message = "Invalid user identity." });

        var query = new GetNotificationsQuery { UserId = userId };
        var result = await mediator.Send(query);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userIdString = currentUserService.UserId;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(new { message = "Invalid user identity." });

        var command = new MarkNotificationAsReadCommand
        {
            NotificationId = id,
            UserId = userId
        };

        var result = await mediator.Send(command);

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
