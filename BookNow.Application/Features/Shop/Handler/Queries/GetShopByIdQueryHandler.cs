using BookNow.Application.DTOs.Shop;
using BookNow.Application.DTOs.Product;
using BookNow.Application.Features.Shop.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Shop.Handler.Queries;

public class GetShopByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetShopByIdQuery, Result<ShopResponseDto>>
{
    public async Task<Result<ShopResponseDto>> Handle(GetShopByIdQuery request, CancellationToken cancellationToken)
    {
        var shop = await unitOfWork.Shops.GetByIdAsync(request.Id, cancellationToken);
        
        if (shop == null)
        {
            return Result<ShopResponseDto>.Failure("Shop not found.");
        }

        var responseDto = new ShopResponseDto
        {
            Id = shop.Id,
            Name = shop.Name,
            Description = shop.Description,
            Address = shop.Address,
            PhoneNumber = shop.PhoneNumber ?? "",
            OpeningHours = shop.OpeningHours ?? "",
            LogoUrl = shop.LogoUrl ?? "",
            Status = shop.Status.ToString(),
            IsSubscribed = shop.IsSubscribed,
            VerifiedAt = shop.VerifiedAt,
            IsVerified = shop.VerifiedAt.HasValue,
            OwnerName = shop.Owner?.FullName ?? "Unknown",
            OwnerEmail = shop.Owner?.Email ?? "No Email",
            TargetProfileId = shop.OwnerId
        };

        var products = await unitOfWork.Products.GetByShopIdAsync(shop.Id, cancellationToken);
        responseDto.Products = products.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            ImageUrls = string.IsNullOrEmpty(p.ImageUrls) ? new List<string>() : p.ImageUrls.Split(',').ToList(),
            Model = p.Model,
            Year = p.Year,
            Brand = p.Brand,
            ShopId = p.ShopId
        }).ToList();

        return Result<ShopResponseDto>.Success(responseDto);
    }
}
