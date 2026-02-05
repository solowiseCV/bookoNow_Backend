
using BookNow.Application.Features.Appointments.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Appointments.Handler.Commands.CreateAppointmentHandler;
public sealed class CreateAppointmentCommandHandler
    : IRequestHandler<CreateAppointmentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateAppointmentCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Client.ToString())
            throw new InvalidOperationException("Only clients can book appointments.");

        var userProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(_currentUser.UserId, ct);

        if (userProfile is null)
        {
             throw new UnauthorizedAccessException("User profile not found.");
        }

        var appointment = new Appointment(
            clientProfileId: userProfile.Id,
            workshopId: request.WorkshopId,
            appointmentAt: request.AppointmentAt,
            issueDescription: request.IssueDescription
        );

        await _unitOfWork.Appointments.AddAsync(appointment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return appointment.Id;
    }
}
