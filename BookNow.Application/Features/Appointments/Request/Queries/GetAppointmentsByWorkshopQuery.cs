using MediatR;
using System.Collections.Generic;

namespace BookNow.Application.Features.Appointments.Request.Queries;

public sealed record GetAppointmentsByWorkshopQuery(Guid WorkshopId) : IRequest<IReadOnlyList<BookNow.Application.DTOs.Appointment.AppointmentDto>>;
