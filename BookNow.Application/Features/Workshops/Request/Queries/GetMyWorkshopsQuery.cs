using BookNow.Application.DTOs.Workshop;
using MediatR;

namespace BookNow.Application.Features.Workshops.Request.Queries;

public sealed record GetMyWorkshopsQuery() : IRequest<IReadOnlyList<WorkshopDto>>;
