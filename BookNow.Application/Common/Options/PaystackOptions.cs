namespace BookNow.Application.Common.Options;

public class PaystackOptions
{
    public const string SectionName = "Paystack";
    public string SecretKey { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.paystack.co";
    public decimal SubscriptionFee { get; set; }
    public decimal CommissionPercentage { get; set; }
}
