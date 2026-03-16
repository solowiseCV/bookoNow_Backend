using BookNow.Domain.Common;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Payment.Request.Commands;

public record InitializePaymentCommand(
    Guid? OrderId, 
    Guid? ShopId, 
    Guid? WorkshopId, 
    PaymentType Type, 
    string Email, 
    decimal Amount, 
    string CallbackUrl) : IRequest<Result<string>>;
