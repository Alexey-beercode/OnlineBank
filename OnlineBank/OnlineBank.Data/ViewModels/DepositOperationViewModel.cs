namespace OnlineBank.Data.ViewModels;

public class DepositOperationViewModel
{
    public Guid DepositId { get; set; }
    public decimal Amount { get; set; }
    public string Note { get; set; }
    public decimal Balance { get; set; }
}