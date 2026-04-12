using BookNow.Application.DTOs.Shop;
using BookNow.Application.DTOs.Product;
using BookNow.Application.Features.Shop.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;
using System.Linq;

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

        var products = await _unitOfWork.Products.GetByShopIdAsync(shop.Id, cancellationToken);
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
