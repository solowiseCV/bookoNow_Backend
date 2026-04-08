
using AutoMapper;
using BookNow.Application.DTOs.Workshop;
using BookNow.Application.Features.Workshops.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using MediatR;

namespace BookNow.Application.Features.Workshops.Handler.Queries;

public sealed class GetNearbyWorkshopsQueryHandler(
    IWorkshopReadRepository workshopReadRepository,
    IMapper mapper)
        : IRequestHandler<GetNearbyWorkshopsQuery, IReadOnlyList<WorkshopDto>>
{
    public async Task<IReadOnlyList<WorkshopDto>> Handle(GetNearbyWorkshopsQuery request, CancellationToken ct)
    {
        var workshops = await workshopReadRepository.GetNearbyAsync(
            request.Latitude,
            request.Longitude,
            request.RadiusKm,
            ct,
            request.IsVerified);

        var dtos = mapper.Map<IEnumerable<WorkshopDto>>(workshops).ToList();
        return dtos;
    }
}


