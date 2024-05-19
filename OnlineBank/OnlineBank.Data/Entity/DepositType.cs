namespace OnlineBank.Data.Entity;

public class DepositType:BaseEntity
{
    public string Name { get; set; }
    public decimal InterestRate { get; set; }
}