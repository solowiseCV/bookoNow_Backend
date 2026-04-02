using BookNow.Application.Features.Product.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Product.Handler.Commands;

public class UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMediaStorageService mediaStorage) : IRequestHandler<UpdateProductCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) return Result<Unit>.Failure("Product not found.");

        var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(request.UserId, cancellationToken);
        if (userProfile == null) return Result<Unit>.Failure("User profile not found.");

        var shop = await unitOfWork.Shops.GetByOwnerIdAsync(userProfile.Id, cancellationToken);
        if (shop == null || product.ShopId != shop.Id)
            return Result<Unit>.Failure("You do not have permission to update this product.");

        var imageUrls = new List<string>();
        if (request.ImageUrlsToKeep != null)
        {
            imageUrls.AddRange(request.ImageUrlsToKeep);
        }

        if (request.NewImages is { Count: > 0 })
        {
            var uploadTasks = request.NewImages.Select(img => mediaStorage.SaveAsync(img, cancellationToken));
            var uploadedUrls = await Task.WhenAll(uploadTasks);
            imageUrls.AddRange(uploadedUrls);
        }

        product.Update(
            request.RequestDto.Name,
            request.RequestDto.Description,
            request.RequestDto.Price,
            string.Join(",", imageUrls),
            request.RequestDto.Model,
            request.RequestDto.Year,
            request.RequestDto.Brand,
            request.RequestDto.PartNumber
        );

        unitOfWork.Products.Update(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value, "Product updated successfully.");
    }
}
