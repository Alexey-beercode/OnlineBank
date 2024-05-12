namespace OnlineBank.Data.Entity;

public class UserRole:BaseEntity
{
    public Guid Userid { get; set; }
    public Guid RoleId { get; set; }
    public bool IsDeleted { get; set; }
}