using BookNow.Application.DTOs.Shop;
using BookNow.Domain.Common;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Shop.Request.Queries;

public record GetAllShopsQuery(ShopStatus? Status = null) : IRequest<Result<IEnumerable<ShopResponseDto>>>;
