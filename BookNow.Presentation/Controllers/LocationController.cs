using BookNow.Presentation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookNow.Presentation.Controllers;

/// <summary>
/// REST fallback: returns the last-known mechanic location for an appointment.
/// Useful when a client reconnects and missed real-time updates.
/// </summary>
[ApiController]
[Route("location")]
[Authorize]
public sealed class LocationController(ILocationCache cache) : ControllerBase
{
    [HttpGet("{appointmentId:guid}")]
    public async Task<IActionResult> GetLastLocation(Guid appointmentId)
    {
        var payload = await cache.GetAsync(appointmentId.ToString());

        if (payload is null)
            return NotFound(new { message = "No location data available for this appointment." });

        return Ok(payload);
    }
}
