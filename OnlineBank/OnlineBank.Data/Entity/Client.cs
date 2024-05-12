namespace OnlineBank.Data.Entity;

public class Client:BaseEntity
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDay { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public bool IsDeleted { get; set; }
}