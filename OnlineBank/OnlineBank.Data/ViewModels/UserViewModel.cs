using OnlineBank.Data.Entity;

namespace OnlineBank.Data.ViewModels;

public class UserViewModel
{
    public User User { get; set; }
    public List<Role> Roles { get; set; }
}