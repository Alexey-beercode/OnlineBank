using OnlineBank.Data.Entity;

namespace OnlineBank.Data.ViewModels;

public class AccountsViewModel
{
    public List<Account> Accounts { get; set; }
    public decimal Balance { get; set; }
}