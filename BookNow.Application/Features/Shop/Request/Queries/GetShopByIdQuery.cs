using BookNow.Application.DTOs.Shop;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Shop.Request.Queries;

public record GetShopByIdQuery(Guid Id) : IRequest<Result<ShopResponseDto>>;
