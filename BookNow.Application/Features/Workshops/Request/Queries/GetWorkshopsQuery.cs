using BookNow.Application.DTOs.Workshop;
using BookNow.Application.Models;
using MediatR;

namespace BookNow.Application.Features.Workshops.Request.Queries;

public sealed record GetWorkshopsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    double? MinRating = null
) : IRequest<PaginatedResult<WorkshopDto>>;
