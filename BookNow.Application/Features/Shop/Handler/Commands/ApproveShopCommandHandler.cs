using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Features.Shop.Request.Commands;
using BookNow.Domain.Common;
using MediatR;
using Shop = BookNow.Domain.Entities.Shop;

namespace BookNow.Application.Features.Shop.Handler.Commands;

public class ApproveShopCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ApproveShopCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ApproveShopCommand request, CancellationToken cancellationToken)
    {
        var shop = await unitOfWork.Shops.GetByIdAsync(request.ShopId, cancellationToken);
        if (shop == null) return Result<bool>.Failure("Shop not found.");

        if (request.Approve)
        {
            shop.Approve();
        }
        else
        {
            shop.Reject();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, request.Approve ? "Shop approved successfully." : "Shop rejected successfully.");
    }
}
