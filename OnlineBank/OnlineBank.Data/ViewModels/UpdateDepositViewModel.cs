using OnlineBank.Data.Entity;

namespace OnlineBank.Data.ViewModels;

public class UpdateDepositViewModel
{
    public Guid DepositId { get; set; }
    public List<DepositType> DepositTypes { get; set; }
    public Guid DepositTypeId { get; set; }
    public decimal Balance { get; set; }
    public decimal InterestRate { get; set; }
    public int MounthCount { get; set; }
}