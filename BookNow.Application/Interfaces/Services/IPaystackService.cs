using BookNow.Application.DTOs.Payment;

namespace BookNow.Application.Interfaces.Services;

public interface IPaystackService
{
    Task<InitializePaymentResponseDto> InitializePaymentAsync(InitializePaymentRequestDto request, CancellationToken ct);
    Task<VerifyPaymentResponseDto> VerifyPaymentAsync(string reference, CancellationToken ct);
    Task<CreateSubaccountResponseDto> CreateSubaccountAsync(CreateSubaccountRequestDto request, CancellationToken ct);
}
