
using MediatR;
using BookNow.Application.Models;

namespace BookNow.Application.Features.Appointments.Request.Commands;
public sealed record CreateAppointmentCommand(
    Guid WorkshopId,
    DateTime AppointmentAt,
    string IssueDescription,
    List<MediaFile>? MediaFiles
) : IRequest<Guid>;
