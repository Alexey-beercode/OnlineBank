using OnlineBank.Data.Entity;

namespace OnlineBank.Data.ViewModels;

public class TransactionViewModel
{
    public Transaction Transaction { get; set; }
    public TransactionType TransactionType { get; set; }
    public string DepositNumber { get; set; }
}