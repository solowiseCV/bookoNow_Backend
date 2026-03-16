using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookNow.Application.Features.Payment.Request.Commands;
using BookNow.Application.DTOs.Workshop;
using BookNow.Domain.Enums;
using Microsoft.Extensions.Configuration;
using BookNow.Application.Interfaces.Persistence;

namespace BookNow.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkshopSubscriptionsController(IMediator mediator, IConfiguration configuration, IUnitOfWork unitOfWork) : ControllerBase
{
    [Authorize(Roles = "Mechanic")]
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] WorkshopSubscribeRequestDto request, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }
        
        var workshop = await unitOfWork.Workshops.GetByIdAsync(request.WorkshopId, cancellationToken);
        if (workshop == null) return NotFound("Workshop not found.");
        
        var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, cancellationToken);
        if (userProfile == null || workshop.MechanicProfileId != userProfile.Id)
        {
            return Forbid("You do not own this workshop.");
        }

        if (workshop.IsSubscribed)
        {
            return BadRequest("Workshop is already subscribed.");
        }

        decimal amount = configuration.GetValue<decimal>("Paystack:SubscriptionFee");
        if (amount <= 0) amount = 5000; // Fallback

        var command = new InitializePaymentCommand(
            null, 
            null, 
            request.WorkshopId, 
            PaymentType.Subscription, 
            request.Email, 
            amount, 
            request.CallbackUrl);

        var result = await mediator.Send(command);
        
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}

