using BookNow.Application.DTOs.Shop;
using BookNow.Application.Features.Shop.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Shop.Handler.Queries;

public class GetShopByOwnerIdQueryHandler : IRequestHandler<GetShopByOwnerIdQuery, Result<ShopResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetShopByOwnerIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ShopResponseDto>> Handle(GetShopByOwnerIdQuery request, CancellationToken cancellationToken)
    {
        var shop = await _unitOfWork.Shops.GetByOwnerIdAsync(request.OwnerId, cancellationToken);
        
        if (shop == null)
        {
            return Result<ShopResponseDto>.Failure("Shop not found.");
        }

        var responseDto = new ShopResponseDto
        {
            Id = shop.Id,
            Name = shop.Name,
            Description = shop.Description,
            LogoUrl = shop.LogoUrl
        };

        return Result<ShopResponseDto>.Success(responseDto);
    }
}
