using BookNow.Application.DTOs.Product;
using BookNow.Application.Features.Product.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Product.Handler.Commands;

public class AddProductCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddProductCommand, Result<ProductResponseDto>>
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

        var product = new BookNow.Domain.Entities.Product(
            request.RequestDto.Name,
            request.RequestDto.Description,
            request.RequestDto.Price,
            request.RequestDto.StockQuantity,
            shop.Id,
            request.RequestDto.ImageUrls
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
            ImageUrls = product.ImageUrls,
            ShopId = product.ShopId
        };

        return Result<ProductResponseDto>.Success(responseDto, "Product added successfully.");
    }
}
