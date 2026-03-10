using AutoMapper;
using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Appointments.Request.Queries;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Enums;
using MediatR;
using System.Collections.Generic;

namespace BookNow.Application.Features.Appointments.Handler.Queries.GetAppointmentsByClientHandler;

public sealed class GetAppointmentsByClientQueryHandler
    : IRequestHandler<GetAppointmentsByClientQuery, IReadOnlyList<BookNow.Application.DTOs.Appointment.AppointmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetAppointmentsByClientQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BookNow.Application.DTOs.Appointment.AppointmentDto>> Handle(GetAppointmentsByClientQuery request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin.ToString())
        {
            if (!Guid.TryParse(_currentUser.UserId, out var userId))
                throw new ForbiddenAccessException("Invalid user identity.");

            var profile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct);
            if (profile is null)
                throw new ForbiddenAccessException("User profile not found.");

            // only clients can view their own list (mechanic/admin maybe not)
            if (_currentUser.Role != UserRole.Client.ToString())
                throw new ForbiddenAccessException("Only clients may view their appointments.");

            var appointments = await _unitOfWork.Appointments.GetByClientAsync(profile.Id, ct);
            return _mapper.Map<IReadOnlyList<BookNow.Application.DTOs.Appointment.AppointmentDto>>(appointments);
        }

        // admin may see all appointments; use repository generic get all
        var all = await _unitOfWork.Appointments.GetAllAsync(ct);
        return _mapper.Map<IReadOnlyList<BookNow.Application.DTOs.Appointment.AppointmentDto>>(all);
    }
}
