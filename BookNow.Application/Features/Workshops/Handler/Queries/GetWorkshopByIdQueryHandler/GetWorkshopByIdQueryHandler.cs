using AutoMapper;
using BookNow.Application.DTOs.Workshop;
using BookNow.Application.Features.Workshops.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using MediatR;
using Workshop = BookNow.Domain.Entities.Workshop;

namespace BookNow.Application.Features.Workshops.Handler.Queries.GetWorkshopByIdQueryHandler;

public sealed class GetWorkshopByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<GetWorkshopByIdQuery, WorkshopDto?>
{
    public async Task<WorkshopDto?> Handle(GetWorkshopByIdQuery request, CancellationToken ct)
    {
        var workshop = await unitOfWork.Workshops.GetByIdAsync(request.Id, ct);
        
        if (workshop == null)
            return null;

        return mapper.Map<WorkshopDto>(workshop);
    }
}
