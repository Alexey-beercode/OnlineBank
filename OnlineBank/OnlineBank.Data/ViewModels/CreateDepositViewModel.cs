using OnlineBank.Data.Entity;

namespace OnlineBank.Data.ViewModels;

public class CreateDepositViewModel
{
    public List<DepositType> DepositTypes { get; set; }
    public decimal Balance { get; set; }
    public int MounthCount { get; set; }
    public string SelectedDepositType { get; set; }
    
}