using MediatR;

namespace BookNow.Application.Features.Appointments.Request.Queries;

public sealed record GetAppointmentByIdQuery(Guid AppointmentId) : IRequest<BookNow.Application.DTOs.Appointment.AppointmentDto>;
