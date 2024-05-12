namespace OnlineBank.Data.Entity;

public class Transaction : BaseEntity
{
    public string Number { get; set; }
    public string Note { get; set; }
    public DateTime Date { get; set; }
    public Guid TypeId { get; set; }
    public decimal Amount { get; set; }
    public bool IsCanceled { get; set; }
    public Guid DepositId { get; set; }

}