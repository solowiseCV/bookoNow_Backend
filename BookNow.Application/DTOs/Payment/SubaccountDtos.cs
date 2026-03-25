namespace BookNow.Application.DTOs.Payment;

public class CreateSubaccountRequestDto
{
    public string BusinessName { get; set; } = null!;
    public string SettlementBank { get; set; } = null!; 
    public string AccountNumber { get; set; } = null!;
    public decimal PercentageCharge { get; set; } = 5; 
}

public class CreateSubaccountResponseDto
{
    public bool Status { get; set; }
    public string Message { get; set; } = null!;
    public SubaccountData? Data { get; set; }
}

public class SubaccountData
{
    public string SubaccountCode { get; set; } = null!;
    public string BusinessName { get; set; } = null!;
}
