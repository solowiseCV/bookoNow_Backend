using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Appointments.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Appointments.Handler.Commands.DeleteAppointmentHandler;

public sealed class DeleteAppointmentCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser) : IRequestHandler<DeleteAppointmentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAppointmentCommand request, CancellationToken ct)
    {
        if (currentUser.Role != UserRole.Admin.ToString())
            throw new ForbiddenAccessException("Only administrators can delete appointments.");

        var appointment = await unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, ct);
        if (appointment is null)
            throw new KeyNotFoundException("Appointment not found.");

        unitOfWork.Appointments.Remove(appointment);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
