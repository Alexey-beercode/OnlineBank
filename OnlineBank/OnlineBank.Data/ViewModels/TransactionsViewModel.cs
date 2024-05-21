namespace OnlineBank.Data.ViewModels;

public class TransactionsViewModel
{
    public Guid DepositNumber { get; set; }
    public List<TransactionViewModel> TransactionViewModels { get; set; }
    public string ClientName { get; set; }
    public string ClientSurname { get; set; }
}