using AutoMapper;
using BookNow.Application.DTOs.Workshop;
using BookNow.Application.Features.Workshops.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using MediatR;

namespace BookNow.Application.Features.Workshops.Handler.Queries.GetWorkshopByIdQueryHandler;

public sealed class GetWorkshopByIdQueryHandler
    : IRequestHandler<GetWorkshopByIdQuery, WorkshopDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetWorkshopByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<WorkshopDto?> Handle(GetWorkshopByIdQuery request, CancellationToken ct)
    {
        var workshop = await _unitOfWork.Workshops.GetByIdAsync(request.Id, ct);
        
        if (workshop == null)
            return null;

        return _mapper.Map<WorkshopDto>(workshop);
    }
}
