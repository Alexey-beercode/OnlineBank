using OnlineBank.Data.Entity;

namespace OnlineBank.Data.ViewModels;

public class AccountWithClientViewModel
{
    public Account Account { get; set; }
    public Client Client { get; set; }
}