using BookNow.Application.Features.Appointments.Request.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookNow.Presentation.Controllers;

[ApiController]
[Route("appointments")]
[Authorize]
public class AppointmentController(ISender _sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentCommand command)
    {
        var appointmentId = await _sender.Send(command);
        return CreatedAtAction(nameof(CreateAppointment), new { id = appointmentId }, new { id = appointmentId });
    }
}
