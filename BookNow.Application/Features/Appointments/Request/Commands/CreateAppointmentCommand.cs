
using MediatR;

namespace BookNow.Application.Features.Appointments.Request.Commands;
public sealed record CreateAppointmentCommand(
    Guid WorkshopId,
    DateTime AppointmentAt,
    string IssueDescription
) : IRequest<Guid>;
