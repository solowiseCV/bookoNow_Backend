using BookNow.Application.DTOs.Workshop;
using MediatR;

namespace BookNow.Application.Features.Workshops.Request.Queries;

public record GetWorkshopByIdQuery(Guid Id) : IRequest<WorkshopDto?>;
