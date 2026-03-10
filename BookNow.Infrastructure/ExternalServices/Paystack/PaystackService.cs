using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookNow.Application.DTOs.Payment;
using BookNow.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

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
            callback_url = request.CallbackUrl
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
}
