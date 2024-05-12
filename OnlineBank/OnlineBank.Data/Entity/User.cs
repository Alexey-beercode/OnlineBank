namespace OnlineBank.Data.Entity;

public class User:BaseEntity
{
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public bool IsDeleted { get; set; }
    public Guid ClientId { get; set; }
}