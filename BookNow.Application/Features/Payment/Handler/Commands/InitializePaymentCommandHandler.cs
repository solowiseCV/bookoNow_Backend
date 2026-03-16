using BookNow.Application.Features.Payment.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Application.DTOs.Payment;
using BookNow.Domain.Common;
using BookNow.Domain.Entities;
using MediatR;
using BookNow.Application.Common.Options;

namespace BookNow.Application.Features.Payment.Handler.Commands;

public class InitializePaymentCommandHandler(
    IUnitOfWork unitOfWork, 
    IPaystackService paystackService,
    Microsoft.Extensions.Options.IOptions<PaystackOptions> paystackOptions) : IRequestHandler<InitializePaymentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(InitializePaymentCommand request, CancellationToken cancellationToken)
    {
        var reference = Guid.NewGuid().ToString();
        var commissionRate = paystackOptions.Value.CommissionPercentage / 100.0m;
        var commission = request.Type == BookNow.Domain.Enums.PaymentType.Order ? request.Amount * commissionRate : 0;

        var payment = new BookNow.Domain.Entities.Payment(
            reference,
            request.Amount,
            commission,
            "Paystack",
            request.Type,
            request.OrderId,
            request.ShopId,
            request.WorkshopId
        );

        await unitOfWork.Payments.AddAsync(payment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var paystackRequest = new InitializePaymentRequestDto
        {
            Email = request.Email,
            Amount = request.Amount,
            Reference = reference,
            CallbackUrl = request.CallbackUrl
        };

        var response = await paystackService.InitializePaymentAsync(paystackRequest, cancellationToken);

        if (response.Status)
        {
            return Result<string>.Success(response.Data.AuthorizationUrl, "Payment initialized successfully.");
        }

        return Result<string>.Failure("Failed to initialize payment with Paystack.");
    }
}
