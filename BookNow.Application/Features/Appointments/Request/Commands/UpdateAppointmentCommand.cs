using MediatR;

namespace BookNow.Application.Features.Appointments.Request.Commands;

public sealed record UpdateAppointmentCommand(
    Guid AppointmentId,
    DateTime AppointmentAt,
    string IssueDescription
) : IRequest<Unit>;
