using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Payment.Request.Commands;

public class VerifyPaymentCommand : IRequest<Result<string>>
{
    public string Reference { get; set; }

    public VerifyPaymentCommand(string reference)
    {
        Reference = reference;
    }
}
