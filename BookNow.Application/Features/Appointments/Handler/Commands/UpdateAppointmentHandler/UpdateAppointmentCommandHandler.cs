using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Appointments.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Appointments.Handler.Commands.UpdateAppointmentHandler;

public sealed class UpdateAppointmentCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<UpdateAppointmentCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAppointmentCommand request, CancellationToken ct)
    {
        // only admin may update
        if (currentUser.Role != UserRole.Admin.ToString())
            throw new ForbiddenAccessException("Only administrators can update appointments.");

        var appointment = await unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, ct);
        if (appointment is null)
            throw new KeyNotFoundException("Appointment not found.");

        // simple update logic, using internal methods or direct assignment
        appointment.UpdateAppointmentTime(request.AppointmentAt); 
        appointment.UpdateIssueDescription(request.IssueDescription);
        unitOfWork.Appointments.Update(appointment);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
