namespace BookNow.Application.DTOs.Payment;

public class CreateSubaccountRequestDto
{
    public string BusinessName { get; set; } = null!;
    public string SettlementBank { get; set; } = null!; // Bank Code
    public string AccountNumber { get; set; } = null!;
    public decimal PercentageCharge { get; set; } = 5; // Platform commission
}

public class CreateSubaccountResponseDto
{
    public bool Status { get; set; }
    public string Message { get; set; } = null!;
    public SubaccountData? Data { get; set; }
}

public class SubaccountData
{
    public string SubaccountCode { get; set; } = null!; // ACCT_xxxx
    public string BusinessName { get; set; } = null!;
}
