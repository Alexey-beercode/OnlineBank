using OnlineBank.Data.Entity;

namespace OnlineBank.Data.ViewModels;

public class UpdateUserViewModel
{
    public Role NewRole { get; set; }
    public List<Role> Roles { get; set; }
    public User User { get; set; }
}