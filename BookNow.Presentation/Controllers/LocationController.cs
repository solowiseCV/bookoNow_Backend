using BookNow.Presentation.Services;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BookNow.Presentation.Models;

namespace BookNow.Presentation.Controllers;


[ApiController]
[Route("location")]
[Authorize]
public sealed class LocationController(ILocationCache cache) : BaseApiController
{
  
    [SwaggerOperation(Summary = "Retrieves the last known location of a mechanic for a specific appointment")]
    [HttpGet("{appointmentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLastLocation(Guid appointmentId)
    {
        var payload = await cache.GetAsync(appointmentId.ToString());

        if (payload is null)
            return NotFound(new ApiResponse(false, "No location data available for this appointment."));

        return Ok(new ApiResponse<object>(true, "Location data retrieved successfully", payload));
    }
}
