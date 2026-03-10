using AutoMapper;
using BookNow.Application.Common.Exceptions;
using BookNow.Application.DTOs.Workshop;
using BookNow.Application.Features.Workshops.Request.Queries;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using MediatR;

namespace BookNow.Application.Features.Workshops.Handler.Queries;

public sealed class GetMyWorkshopsQueryHandler
    : IRequestHandler<GetMyWorkshopsQuery, IReadOnlyList<WorkshopDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetMyWorkshopsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<WorkshopDto>> Handle(GetMyWorkshopsQuery request, CancellationToken ct)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new ForbiddenAccessException("Invalid user identity.");

        var userProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct)
            ?? throw new ForbiddenAccessException("User profile not found.");

        var workshops = await _unitOfWork.Workshops.GetByMechanicAsync(userProfile.Id, ct);
        return _mapper.Map<IReadOnlyList<WorkshopDto>>(workshops);
    }
}
