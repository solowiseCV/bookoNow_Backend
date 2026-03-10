using BookNow.Application.DTOs.Shop;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Shop.Request.Queries;

public class GetShopByOwnerIdQuery : IRequest<Result<ShopResponseDto>>
{
    public Guid OwnerId { get; set; }

    public GetShopByOwnerIdQuery(Guid ownerId)
    {
        OwnerId = ownerId;
    }
}
