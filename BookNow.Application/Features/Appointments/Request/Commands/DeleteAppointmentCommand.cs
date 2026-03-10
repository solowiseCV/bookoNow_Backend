using MediatR;

namespace BookNow.Application.Features.Appointments.Request.Commands;

public sealed record DeleteAppointmentCommand(Guid AppointmentId) : IRequest<Unit>;
