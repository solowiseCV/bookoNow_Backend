using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Appointments.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Appointments.Handler.Commands.UpdateAppointmentHandler;

public sealed class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateAppointmentCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdateAppointmentCommand request, CancellationToken ct)
    {
        // only admin may update
        if (_currentUser.Role != UserRole.Admin.ToString())
            throw new ForbiddenAccessException("Only administrators can update appointments.");

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, ct);
        if (appointment is null)
            throw new KeyNotFoundException("Appointment not found.");

        // simple update logic, using internal methods or direct assignment
        appointment.UpdateAppointmentTime(request.AppointmentAt); // we need to implement maybe
        appointment.UpdateIssueDescription(request.IssueDescription);
        _unitOfWork.Appointments.Update(appointment);
        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
