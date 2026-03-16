
using BookNow.Application.Features.Appointments.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;
using MediatR;
using Appointment = BookNow.Domain.Entities.Appointment;

namespace BookNow.Application.Features.Appointments.Handler.Commands.CreateAppointmentHandler;
public sealed class CreateAppointmentCommandHandler
    : IRequestHandler<CreateAppointmentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediaStorageService _mediaStorage;

    public CreateAppointmentCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMediaStorageService mediaStorage)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mediaStorage = mediaStorage;
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

        return appointment.Id;
    }
}
