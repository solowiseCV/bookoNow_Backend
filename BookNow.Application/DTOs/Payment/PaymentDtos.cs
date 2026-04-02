namespace BookNow.Application.DTOs.Payment;

public class InitializePaymentRequestDto
{
    public decimal Amount { get; set; } 
    public string Email { get; set; } = null!;
    public string Reference { get; set; } = null!;
    public string CallbackUrl { get; set; } = null!;
    public string? Subaccount { get; set; } 
    public int? TransactionCharge { get; set; } 
}

public class InitializePaymentResponseDto
{
    public bool Status { get; set; }
    public string Message { get; set; } = null!;
    public PaystackInitializeData? Data { get; set; }
}

public class PaystackInitializeData
{
    public string AuthorizationUrl { get; set; } = null!;
    public string AccessCode { get; set; } = null!;
    public string Reference { get; set; } = null!;
}

public class VerifyPaymentResponseDto
{
    public bool Status { get; set; }
    public string Message { get; set; } = null!;
    public PaystackVerifyData? Data { get; set; }
}

public class PaystackVerifyData
{
    public string Status { get; set; } = null!; 
    public string Reference { get; set; } = null!;
    public decimal Amount { get; set; }
}
