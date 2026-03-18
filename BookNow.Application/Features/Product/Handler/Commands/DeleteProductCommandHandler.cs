using BookNow.Application.Features.Product.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Product.Handler.Commands;

public class DeleteProductCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteProductCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null) return Result<Unit>.Failure("Product not found.");

        var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(request.UserId, cancellationToken);
        if (userProfile == null) return Result<Unit>.Failure("User profile not found.");

        var shop = await unitOfWork.Shops.GetByOwnerIdAsync(userProfile.Id, cancellationToken);
        if (shop == null || product.ShopId != shop.Id)
            return Result<Unit>.Failure("You do not have permission to delete this product.");

        unitOfWork.Products.Delete(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value, "Product deleted successfully.");
    }
}
