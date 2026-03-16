using BookNow.Application.DTOs.Payment;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Common;
using MediatR;
using Workshop = BookNow.Domain.Entities.Workshop;
using BookNow.Application.Common.Options;

namespace BookNow.Application.Features.Workshop.Handler.Commands
{
    public record RegisterWorkshopSubaccountCommand(
        Guid UserId,
        Guid WorkshopId,
        string BankName,
        string BankCode,
        string AccountNumber,
        string AccountName) : IRequest<Result<string>>;
}

namespace BookNow.Application.Features.Workshop.Handler.Commands
{
    public class RegisterWorkshopSubaccountCommandHandler(
        IUnitOfWork unitOfWork, 
        IPaystackService paystackService,
        Microsoft.Extensions.Options.IOptions<PaystackOptions> paystackOptions) 
            : IRequestHandler<RegisterWorkshopSubaccountCommand, Result<string>>
    {

        public async Task<Result<string>> Handle(RegisterWorkshopSubaccountCommand request, CancellationToken cancellationToken)
        {
            var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(request.UserId, cancellationToken);
            if (userProfile == null) return Result<string>.Failure("User profile not found.");

            var workshop = await unitOfWork.Workshops.GetByIdAsync(request.WorkshopId, cancellationToken);
            if (workshop == null) return Result<string>.Failure("Workshop not found.");

            if (workshop.MechanicProfileId != userProfile.Id)
                return Result<string>.Failure("You do not own this workshop.");

            if (!string.IsNullOrEmpty(workshop.PaystackSubaccountCode))
                return Result<string>.Failure("Workshop already has a registered subaccount.");

            // Call Paystack to create subaccount
            var paystackRequest = new CreateSubaccountRequestDto
            {
                BusinessName = workshop.Name,
                SettlementBank = request.BankCode,
                AccountNumber = request.AccountNumber,
                PercentageCharge = paystackOptions.Value.CommissionPercentage
            };

            var response = await paystackService.CreateSubaccountAsync(paystackRequest, cancellationToken);
            if (!response.Status || response.Data == null)
            {
                return Result<string>.Failure(response.Message);
            }

            workshop.SetBankDetails(request.BankName, request.BankCode, request.AccountNumber, request.AccountName);
            workshop.SetPaystackSubaccount(response.Data.SubaccountCode);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(response.Data.SubaccountCode, "Subaccount registered successfully.");
        }
    }
}
