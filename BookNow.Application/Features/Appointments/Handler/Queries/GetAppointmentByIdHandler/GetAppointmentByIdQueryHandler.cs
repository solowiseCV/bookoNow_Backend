using AutoMapper;
using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Appointments.Request.Queries;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Appointments.Handler.Queries.GetAppointmentByIdHandler;

public sealed class GetAppointmentByIdQueryHandler
    : IRequestHandler<GetAppointmentByIdQuery, BookNow.Application.DTOs.Appointment.AppointmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetAppointmentByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<BookNow.Application.DTOs.Appointment.AppointmentDto> Handle(GetAppointmentByIdQuery request, CancellationToken ct)
    {
        // load appointment including attachments
        var appointment = await _unitOfWork.Appointments.GetWithAttachmentsByIdAsync(request.AppointmentId, ct);
        if (appointment is null)
            throw new KeyNotFoundException("Appointment not found.");

        // authorization: admin or client who owns the appointment or mechanic of workshop
        if (_currentUser.Role != UserRole.Admin.ToString())
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                throw new ForbiddenAccessException("Invalid user identity.");

            var profile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct);
            if (profile is null)
                throw new ForbiddenAccessException("User profile not found.");

            var ownAppointment = appointment.ClientProfileId == profile.Id;
            if (!ownAppointment)
            {
                // check mechanic ownership
                var isMechanic = _currentUser.Role == UserRole.Mechanic.ToString();
                if (isMechanic)
                {
                    var ownsWorkshop = await _unitOfWork.Workshops.IsOwnedByMechanicAsync(appointment.WorkshopId, profile.Id, ct);
                    if (!ownsWorkshop)
                        throw new ForbiddenAccessException("Not allowed to view this appointment.");
                }
                else
                {
                    throw new ForbiddenAccessException("Not allowed to view this appointment.");
                }
            }
        }

        return _mapper.Map<BookNow.Application.DTOs.Appointment.AppointmentDto>(appointment);
    }
}

