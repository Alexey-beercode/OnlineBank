using OnlineBank.Data.Entity;

namespace OnlineBank.Data.ViewModels;

public class DepositByClienViewModel
{
    public Deposit Deposit { get; set; }
    public DepositType DepositType {get; set;}
}