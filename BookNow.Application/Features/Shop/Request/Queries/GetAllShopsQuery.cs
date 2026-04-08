using BookNow.Application.DTOs.Shop;
using BookNow.Domain.Common;
using BookNow.Domain.Enums;
using MediatR;

using BookNow.Application.Models;

namespace BookNow.Application.Features.Shop.Request.Queries;

public record GetAllShopsQuery(
    ShopStatus? Status = null,
    bool? IsVerified = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedResult<ShopResponseDto>>>;
