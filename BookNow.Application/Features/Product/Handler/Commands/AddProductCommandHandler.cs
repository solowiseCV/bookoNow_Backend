using BookNow.Application.DTOs.Product;
using BookNow.Application.Features.Product.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Enums;

namespace BookNow.Application.Features.Product.Handler.Commands;

public class AddProductCommandHandler(IUnitOfWork unitOfWork, IMediaStorageService mediaStorage) : IRequestHandler<AddProductCommand, Result<ProductResponseDto>>
{
    public async Task<Result<ProductResponseDto>> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        // Get user profile first
        var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(request.UserId, cancellationToken);
        if (userProfile == null) return Result<ProductResponseDto>.Failure("User profile not found.");
        
        // Check if user owns a shop
        var shop = await unitOfWork.Shops.GetByOwnerIdAsync(userProfile.Id, cancellationToken);
        if (shop == null)
            return Result<ProductResponseDto>.Failure("User does not own a shop.");

        // Check if shop is verified
        if (shop.Status != ShopStatus.Verified)
            return Result<ProductResponseDto>.Failure("Shop must be verified before adding products.");

        // Check product limits
        var productCount = await unitOfWork.Products.GetCountByShopIdAsync(shop.Id, cancellationToken);
        var limit = shop.IsSubscribed ? 30 : 5;

        if (productCount >= limit)
            return Result<ProductResponseDto>.Failure($"Product limit reached. Your current limit is {limit}. Upgrade your subscription to add more.");

        string? imageUrls = null;
        if (request.Images is { Count: > 0 })
        {
            var uploadTasks = request.Images.Select(img => mediaStorage.SaveAsync(img, cancellationToken));
            var uploadedUrls = await Task.WhenAll(uploadTasks);
            imageUrls = string.Join(",", uploadedUrls);
        }

        var product = new BookNow.Domain.Entities.Product(
            request.RequestDto.Name,
            request.RequestDto.Description,
            request.RequestDto.Price,
            request.RequestDto.StockQuantity,
            shop.Id,
            imageUrls ?? string.Empty,
            request.RequestDto.Model,
            request.RequestDto.Year,
            request.RequestDto.Brand ,
            request.RequestDto.PartNumber
        );

        await unitOfWork.Products.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var responseDto = new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrls = string.IsNullOrEmpty(product.ImageUrls) ? new List<string>() : product.ImageUrls.Split(',').ToList(),
            Model = product.Model,
            Year = product.Year,
            Brand = product.Brand,
            ShopId = product.ShopId,
            PartNumber = product.PartNumber
        };

        return Result<ProductResponseDto>.Success(responseDto, "Product added successfully.");
    }
}
