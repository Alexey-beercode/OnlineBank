using OnlineBank.Data.Entity;

namespace OnlineBank.Data.ViewModels;

public class UsersViewModel
{
    public List<UserViewModel> UserViewModels { get; set; }
    public List<Role> Roles { get; set; }
}