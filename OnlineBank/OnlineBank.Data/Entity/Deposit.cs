namespace OnlineBank.Data.Entity;

public class Deposit:BaseEntity
{
    public string Number { get; set; }
    public Guid TypeId { get; set; }
    public decimal Balance { get; set; }
    public Guid ClientId { get; set; }
    public TimeSpan Time { get; set; }
    public decimal InterestRate { get; set; }
    public bool IsClosed { get; set; }
}