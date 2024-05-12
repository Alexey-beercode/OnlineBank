namespace OnlineBank.Data.Entity;

public class Account:BaseEntity
{
    public decimal Balance { get; set; }
    public string Number { get; set; }
    public bool IsClosed { get; set; }
    public Guid ClientId { get; set; }
}