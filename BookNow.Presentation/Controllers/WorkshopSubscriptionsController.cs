using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookNow.Application.Features.Payment.Request.Commands;
using BookNow.Presentation.Models;

namespace BookNow.Presentation.Controllers;

[Route("workshop-subscriptions")]
[ApiController]
public class WorkshopSubscriptionsController(IMediator mediator) : BaseApiController
{
  
    [SwaggerOperation(Summary = "Initiates a subscription process for a workshop. Restricted to users with 'Mechanic' role")]
    [Authorize(Roles = "Mechanic")]
    [HttpPost("subscribe")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Subscribe([FromBody] BookNow.Application.DTOs.Workshop.WorkshopSubscribeRequestDto request)
    {
        var command = new InitializeSubscriptionCommand(UserId, null, request.WorkshopId, request.Email, request.CallbackUrl);
        var result = await mediator.Send(command);
        return HandleResult(result);
    }
}

