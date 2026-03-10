using AutoMapper;
using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Appointments.Request.Queries;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Enums;
using MediatR;
using System.Collections.Generic;

namespace BookNow.Application.Features.Appointments.Handler.Queries.GetAppointmentsByWorkshopHandler;

public sealed class GetAppointmentsByWorkshopQueryHandler
    : IRequestHandler<GetAppointmentsByWorkshopQuery, IReadOnlyList<BookNow.Application.DTOs.Appointment.AppointmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetAppointmentsByWorkshopQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BookNow.Application.DTOs.Appointment.AppointmentDto>> Handle(GetAppointmentsByWorkshopQuery request, CancellationToken ct)
    {
        // authorization: only admin or mechanic owner of the workshop
        if (_currentUser.Role != UserRole.Admin.ToString())
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                throw new ForbiddenAccessException("Invalid user identity.");

            var profile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct);
            if (profile is null)
                throw new ForbiddenAccessException("User profile not found.");

            var owns = await _unitOfWork.Workshops.IsOwnedByMechanicAsync(request.WorkshopId, profile.Id, ct);
            if (!owns)
                throw new ForbiddenAccessException("Not allowed to view appointments for this workshop.");
        }

        var appointments = await _unitOfWork.Appointments.GetByWorkshopAsync(request.WorkshopId, ct);
        return _mapper.Map<IReadOnlyList<BookNow.Application.DTOs.Appointment.AppointmentDto>>(appointments);
    }
}

