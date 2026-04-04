using AutoMapper;
using BookNow.Application.DTOs.Workshop;
using BookNow.Application.Features.Workshops.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Models;
using MediatR;

namespace BookNow.Application.Features.Workshops.Handler.Queries;

public sealed class GetWorkshopsQueryHandler
    : IRequestHandler<GetWorkshopsQuery, PaginatedResult<WorkshopDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetWorkshopsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<WorkshopDto>> Handle(GetWorkshopsQuery request, CancellationToken ct)
    {
        var (items, totalCount) = await _unitOfWork.Workshops.GetPaginatedAsync(request.PageNumber, request.PageSize, ct, request.MinRating, request.Search, request.Type, isVerified: true);

        var dtos = _mapper.Map<IEnumerable<WorkshopDto>>(items).ToList();

        return new PaginatedResult<WorkshopDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
