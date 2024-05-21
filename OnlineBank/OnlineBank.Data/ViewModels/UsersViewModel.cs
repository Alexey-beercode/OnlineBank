using OnlineBank.Data.Entity;

namespace OnlineBank.Data.ViewModels;

public class UsersViewModel
{
    public UsersViewModel()
    {
        UserViewModels = new List<UserViewModel>();
        Roles = new List<Role>();
    }
    public List<UserViewModel> UserViewModels { get; set; }
    public List<Role> Roles { get; set; }
}