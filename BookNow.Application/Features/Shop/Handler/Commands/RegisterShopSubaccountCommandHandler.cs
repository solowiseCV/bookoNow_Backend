using BookNow.Application.DTOs.Payment;
using BookNow.Application.Features.Shop.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Common;
using MediatR;
using Microsoft.Extensions.Options;
using BookNow.Application.Common.Options;
using Shop = BookNow.Domain.Entities.Shop;

namespace BookNow.Application.Features.Shop.Request.Commands
{
    public record RegisterShopSubaccountCommand(
        Guid UserId,
        Guid ShopId,
        string BankName,
        string BankCode,
        string AccountNumber,
        string AccountName) : IRequest<Result<string>>;
}

namespace BookNow.Application.Features.Shop.Handler.Commands
{
    public class RegisterShopSubaccountCommandHandler(
        IUnitOfWork unitOfWork, 
        IPaystackService paystackService,
        IOptions<PaystackOptions> paystackOptions) : IRequestHandler<RegisterShopSubaccountCommand, Result<string>>
    {

        public async Task<Result<string>> Handle(RegisterShopSubaccountCommand request, CancellationToken cancellationToken)
        {
            var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(request.UserId, cancellationToken);
            if (userProfile == null) return Result<string>.Failure("User profile not found.");

            var shop = await unitOfWork.Shops.GetByIdAsync(request.ShopId, cancellationToken);
            if (shop == null) return Result<string>.Failure("Shop not found.");

            if (shop.OwnerId != userProfile.Id)
                return Result<string>.Failure("You do not own this shop.");

            if (!string.IsNullOrEmpty(shop.PaystackSubaccountCode))
                return Result<string>.Failure("Shop already has a registered subaccount.");

            // Call Paystack to create subaccount
            var paystackRequest = new CreateSubaccountRequestDto
            {
                BusinessName = shop.Name,
                SettlementBank = request.BankCode,
                AccountNumber = request.AccountNumber,
                PercentageCharge = paystackOptions.Value.CommissionPercentage
            };

            var response = await paystackService.CreateSubaccountAsync(paystackRequest, cancellationToken);
            if (!response.Status || response.Data == null)
            {
                return Result<string>.Failure(response.Message);
            }

            shop.SetBankDetails(request.BankName, request.BankCode, request.AccountNumber, request.AccountName);
            shop.SetPaystackSubaccount(response.Data.SubaccountCode);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(response.Data.SubaccountCode, "Subaccount registered successfully.");
        }
    }
}
