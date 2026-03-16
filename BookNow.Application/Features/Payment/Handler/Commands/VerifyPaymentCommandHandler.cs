using BookNow.Application.Features.Payment.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Common;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Payment.Handler.Commands;

public class VerifyPaymentCommandHandler : IRequestHandler<VerifyPaymentCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaystackService _paystackService;

    public VerifyPaymentCommandHandler(IUnitOfWork unitOfWork, IPaystackService paystackService)
    {
        _unitOfWork = unitOfWork;
        _paystackService = paystackService;
    }

    public async Task<Result<string>> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Payments.GetByReferenceAsync(request.Reference, cancellationToken);
        if (payment == null) return Result<string>.Failure("Payment record not found.");

        if (payment.Status == PaymentStatus.Success) return Result<string>.Success("Success", "Payment already verified.");

        var response = await _paystackService.VerifyPaymentAsync(request.Reference, cancellationToken);

        if (response.Status && response.Data?.Status == "success")
        {
            payment.MarkAsSuccessful();
            
            if (payment.Type == PaymentType.Order && payment.OrderId.HasValue)
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(payment.OrderId.Value, cancellationToken);
                if (order != null)
                {
                    order.UpdateStatus(OrderStatus.Paid);
                    _unitOfWork.Orders.Update(order);
                }
            }
            else if (payment.Type == PaymentType.Subscription)
            {
                if (payment.ShopId.HasValue)
                {
                    var shop = await _unitOfWork.Shops.GetByIdAsync(payment.ShopId.Value, cancellationToken);
                    if (shop != null)
                    {
                        shop.SetSubscription(true);
                        _unitOfWork.Shops.Update(shop);
                    }
                }
                else if (payment.WorkshopId.HasValue)
                {
                    var workshop = await _unitOfWork.Workshops.GetByIdAsync(payment.WorkshopId.Value, cancellationToken);
                    if (workshop != null)
                    {
                        workshop.SetSubscription(true);
                        _unitOfWork.Workshops.Update(workshop);
                    }
                }
            }

            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Success", "Payment verified successfully.");
        }
        
        payment.MarkAsFailed();
        _unitOfWork.Payments.Update(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Failure("Payment verification failed.");
    }
}
