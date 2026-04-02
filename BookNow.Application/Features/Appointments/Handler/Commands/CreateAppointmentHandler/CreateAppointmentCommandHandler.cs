
using BookNow.Application.Features.Appointments.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Appointments.Handler.Commands.CreateAppointmentHandler;
public sealed class CreateAppointmentCommandHandler
    : IRequestHandler<CreateAppointmentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediaStorageService _mediaStorage;
    // private readonly INotificationService _notificationService;

    public CreateAppointmentCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMediaStorageService mediaStorage
       )
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mediaStorage = mediaStorage;
        // _notificationService = notificationService;
    }

    public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Client.ToString())
            throw new InvalidOperationException("Only clients can book appointments.");

        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity.");

        var userProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct);

        if (userProfile is null)
        {
             throw new UnauthorizedAccessException("User profile not found.");
        }

        var appointment = new BookNow.Domain.Entities.Appointment(
            clientProfileId: userProfile.Id,
            workshopId: request.WorkshopId,
            appointmentAt: request.AppointmentAt,
            issueDescription: request.IssueDescription
        );

        if (request.MediaFiles != null && request.MediaFiles.Any())
        {
            foreach (var file in request.MediaFiles)
            {
                var url = await _mediaStorage.SaveAsync(file, ct);
                appointment.AddAttachment(url, MediaType.Image);
            }
        }

        await _unitOfWork.Appointments.AddAsync(appointment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var workshop = await _unitOfWork.Workshops.GetByIdAsync(request.WorkshopId, ct);
        if (workshop != null)
        {
            var mechanic = await _unitOfWork.UserProfiles.GetByIdAsync(workshop.MechanicProfileId, ct);
            if (mechanic != null)
            {
                var message = $"A new appointment has been booked at your workshop '{workshop.Name}' for {appointment.AppointmentAt}.";
                // await _notificationService.SendNotificationAsync(mechanic.IdentityUserId, workshop.PhoneNumber, message, ct);
            }
        }

        return appointment.Id;
    }
}
