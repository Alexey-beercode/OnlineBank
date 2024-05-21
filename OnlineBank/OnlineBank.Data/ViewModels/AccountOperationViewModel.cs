namespace OnlineBank.Data.ViewModels;

public class AccountOperationViewModel
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Note { get; set; }
}