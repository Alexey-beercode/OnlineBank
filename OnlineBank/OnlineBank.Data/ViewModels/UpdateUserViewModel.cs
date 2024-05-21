using OnlineBank.Data.Entity;

namespace OnlineBank.Data.ViewModels;

public class UpdateUserViewModel
{
    public Guid NewRoleId { get; set; }
    public List<Role> Roles { get; set; }
    public User User { get; set; }
}