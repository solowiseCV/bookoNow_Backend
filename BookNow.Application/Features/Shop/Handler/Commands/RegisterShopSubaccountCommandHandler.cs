using BookNow.Application.DTOs.Payment;
using BookNow.Application.Features.Shop.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Common;
using MediatR;
using Shop = BookNow.Domain.Entities.Shop;

namespace BookNow.Application.Features.Shop.Request.Commands
{
    public record RegisterShopSubaccountCommand(
        Guid UserId,
        string BankName,
        string BankCode,
        string AccountNumber,
        string AccountName) : IRequest<Result<string>>;
}

namespace BookNow.Application.Features.Shop.Handler.Commands
{
    public class RegisterShopSubaccountCommandHandler : IRequestHandler<RegisterShopSubaccountCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaystackService _paystackService;

        public RegisterShopSubaccountCommandHandler(IUnitOfWork unitOfWork, IPaystackService paystackService)
        {
            _unitOfWork = unitOfWork;
            _paystackService = paystackService;
        }

        public async Task<Result<string>> Handle(RegisterShopSubaccountCommand request, CancellationToken cancellationToken)
        {
            var userProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(request.UserId, cancellationToken);
            if (userProfile == null) return Result<string>.Failure("User profile not found.");

            BookNow.Domain.Entities.Shop? shop = await _unitOfWork.Shops.GetByOwnerIdAsync(request.UserId, cancellationToken);
            if (shop == null) return Result<string>.Failure("Shop not found.");

            // Call Paystack to create subaccount
            var paystackRequest = new CreateSubaccountRequestDto
            {
                BusinessName = shop.Name,
                SettlementBank = request.BankCode,
                AccountNumber = request.AccountNumber,
                PercentageCharge = 5 // Platform commission
            };

            var response = await _paystackService.CreateSubaccountAsync(paystackRequest, cancellationToken);
            if (!response.Status || response.Data == null)
            {
                return Result<string>.Failure(response.Message);
            }

            shop.SetBankDetails(request.BankName, request.BankCode, request.AccountNumber, request.AccountName);
            shop.SetPaystackSubaccount(response.Data.SubaccountCode);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(response.Data.SubaccountCode, "Subaccount registered successfully.");
        }
    }
}
