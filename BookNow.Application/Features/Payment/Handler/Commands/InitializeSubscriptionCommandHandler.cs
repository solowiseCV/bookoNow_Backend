using BookNow.Application.Common.Options;
using BookNow.Application.Features.Payment.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using BookNow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace BookNow.Application.Features.Payment.Handler.Commands;

public class InitializeSubscriptionCommandHandler(
    IMediator mediator,
    IUnitOfWork unitOfWork,
    IOptions<PaystackOptions> paystackOptions) : IRequestHandler<InitializeSubscriptionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(InitializeSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(request.UserId, cancellationToken);
        if (userProfile == null) return Result<string>.Failure("User profile not found.");

        decimal amount;
        Guid? shopId = null;
        Guid? workshopId = null;

        if (request.ShopId.HasValue)
        {
            var shop = await unitOfWork.Shops.GetByIdAsync(request.ShopId.Value, cancellationToken);
            if (shop == null) return Result<string>.Failure("Shop not found.");
            if (shop.OwnerId != userProfile.Id) return Result<string>.Failure("You do not own this shop.");
            if (shop.IsSubscribed) return Result<string>.Failure("Shop is already subscribed.");

            amount = paystackOptions.Value.ShopSubscriptionFee > 0 ? paystackOptions.Value.ShopSubscriptionFee : 10000;
            shopId = request.ShopId;
        }
        else if (request.WorkshopId.HasValue)
        {
            var workshop = await unitOfWork.Workshops.GetByIdAsync(request.WorkshopId.Value, cancellationToken);
            if (workshop == null) return Result<string>.Failure("Workshop not found.");
            if (workshop.MechanicProfileId != userProfile.Id) return Result<string>.Failure("You do not own this workshop.");
            if (workshop.IsSubscribed) return Result<string>.Failure("Workshop is already subscribed.");

            amount = paystackOptions.Value.SubscriptionFee > 0 ? paystackOptions.Value.SubscriptionFee : 5000;
            workshopId = request.WorkshopId;
        }
        else
        {
            return Result<string>.Failure("ShopId or WorkshopId must be provided.");
        }

        var command = new InitializePaymentCommand(
            null,
            shopId,
            workshopId,
            PaymentType.Subscription,
            request.Email,
            amount,
            request.CallbackUrl);

        return await mediator.Send(command, cancellationToken);
    }
}
