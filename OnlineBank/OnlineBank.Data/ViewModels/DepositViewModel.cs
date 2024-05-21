using OnlineBank.Data.Entity;

namespace OnlineBank.Data.ViewModels;

public class DepositViewModel
{
    public List<DepositByClienViewModel> Deposits { get; set; }
    public string CLientName { get; set; }
    public string ClientSurname { get; set; }
}