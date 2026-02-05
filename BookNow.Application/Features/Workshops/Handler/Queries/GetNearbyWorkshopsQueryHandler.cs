
using AutoMapper;
using BookNow.Application.DTOs.Workshop;
using BookNow.Application.Features.Workshops.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using MediatR;

namespace BookNow.Application.Features.Workshops.Handler.Queries;

public sealed class GetNearbyWorkshopsQueryHandler
    : IRequestHandler<GetNearbyWorkshopsQuery, IReadOnlyList<WorkshopDto>>
{
    private readonly IWorkshopReadRepository _workshopReadRepository;
    private readonly IMapper _mapper;

    public GetNearbyWorkshopsQueryHandler(
        IWorkshopReadRepository workshopReadRepository,
        IMapper mapper)
    {
        _workshopReadRepository = workshopReadRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<WorkshopDto>> Handle(GetNearbyWorkshopsQuery request, CancellationToken ct)
    {
        var workshops = await _workshopReadRepository.GetNearbyAsync(
            request.Latitude,
            request.Longitude,
            request.RadiusKm,
            ct);

        return _mapper.Map<IReadOnlyList<WorkshopDto>>(workshops);
    }
}


