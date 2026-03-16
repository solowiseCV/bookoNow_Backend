using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookNow.Application.DTOs.Payment;
using BookNow.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using BookNow.Application.Common.Options;

namespace BookNow.Infrastructure.ExternalServices.Paystack;

public class PaystackService : IPaystackService
{
    private readonly HttpClient _httpClient;
    private readonly PaystackOptions _options;

    public PaystackService(HttpClient httpClient, IOptions<PaystackOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.SecretKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<InitializePaymentResponseDto> InitializePaymentAsync(InitializePaymentRequestDto request, CancellationToken ct)
    {
        var paystackRequest = new
        {
            email = request.Email,
            amount = Math.Round(request.Amount * 100), // convert Naira to kobo
            reference = request.Reference,
            callback_url = request.CallbackUrl,
            subaccount = request.Subaccount,
            transaction_charge = request.TransactionCharge
        };

        var response = await _httpClient.PostAsJsonAsync("/transaction/initialize", paystackRequest, ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<InitializePaymentResponseDto>(cancellationToken: ct);

        return result ?? throw new Exception("Failed to deserialize Paystack initialize response.");
    }

    public async Task<VerifyPaymentResponseDto> VerifyPaymentAsync(string reference, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"/transaction/verify/{reference}", ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VerifyPaymentResponseDto>(cancellationToken: ct);

        return result ?? throw new Exception("Failed to deserialize Paystack verify response.");
    }

    public async Task<CreateSubaccountResponseDto> CreateSubaccountAsync(CreateSubaccountRequestDto request, CancellationToken ct)
    {
        var paystackRequest = new
        {
            business_name = request.BusinessName,
            settlement_bank = request.SettlementBank,
            account_number = request.AccountNumber,
            percentage_charge = request.PercentageCharge
        };

        var response = await _httpClient.PostAsJsonAsync("/subaccount", paystackRequest, ct);
        if (!response.IsSuccessStatusCode)
        {
            return new CreateSubaccountResponseDto { Status = false, Message = "Failed to create subaccount on Paystack." };
        }

        var result = await response.Content.ReadFromJsonAsync<CreateSubaccountResponseDto>(cancellationToken: ct);
        return result ?? new CreateSubaccountResponseDto { Status = false, Message = "Failed to parse subaccount response." };
    }
}
