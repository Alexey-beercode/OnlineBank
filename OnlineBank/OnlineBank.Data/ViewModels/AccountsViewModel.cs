using OnlineBank.Data.Entity;

namespace OnlineBank.Data.ViewModels;

public class AccountsViewModel
{
    public List<Account> Accounts { get; set; }
    public string CLientName { get; set; }
    public string ClientSurname { get; set; }
}