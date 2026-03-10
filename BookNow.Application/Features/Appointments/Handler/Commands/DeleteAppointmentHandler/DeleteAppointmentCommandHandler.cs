using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Appointments.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Appointments.Handler.Commands.DeleteAppointmentHandler;

public sealed class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeleteAppointmentCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteAppointmentCommand request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin.ToString())
            throw new ForbiddenAccessException("Only administrators can delete appointments.");

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, ct);
        if (appointment is null)
            throw new KeyNotFoundException("Appointment not found.");

        _unitOfWork.Appointments.Remove(appointment);
        await _unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
