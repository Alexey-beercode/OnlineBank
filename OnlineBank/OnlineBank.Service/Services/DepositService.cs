using System.Text;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.Data.ViewModels;
using OnlineBank.DataManagment.Repositories.Implementations;
using OnlineBank.Service.Exceptions;
using OnlineBank.Service.Service;

namespace OnlineBank.Service.Services;

public class DepositService
{
    private readonly DepositRepository _depositRepository;
    private readonly DepositTypeRepository _depositTypeRepository;
    private readonly TransactionService _transactionService;
    private readonly ClientService _clientService;

    public DepositService(DepositRepository depositRepository, DepositTypeRepository depositTypeRepository, TransactionService transactionService, ClientService clientService)
    {
        _depositRepository = depositRepository;
        _depositTypeRepository = depositTypeRepository;
        _transactionService = transactionService;
        _clientService = clientService;
    }
    
    public async Task<Deposit> GetById(Guid id)
    {
        var deposit = await _depositRepository.GetById(id);
        if (deposit is null)
        {
            throw new Exception("Депозит не найден");
        }

        return deposit;
    }
    public async Task<DepositViewModel> GetDepositsByClientAsync(Guid clientId)
    {
        var client = await _clientService.GetByIdAsync(clientId.ToString());
        var depositsByClient = await _depositRepository.GetByClientId(clientId);
        var depositByClientViewModels = new DepositViewModel();
        
        depositByClientViewModels.Deposits = new List<DepositByClienViewModel>();
        foreach (var depositByClient in depositsByClient)
        {
            var depositType = await _depositTypeRepository.GetById(depositByClient.TypeId);
            var totalAmountAtEnd = await CalculateTotalAmount(depositByClient.Balance,
                (int)(depositByClient.Time.TotalDays / 30), depositType.Name);
            var depositByClientViewModel = new DepositByClienViewModel()
                { Deposit = depositByClient, DepositType = depositType ,TotalAmountAtEnd = totalAmountAtEnd};
           depositByClientViewModels.Deposits.Add(depositByClientViewModel);
        }

        depositByClientViewModels.ClientSurname = client.Surname;
        depositByClientViewModels.CLientName = client.Name;

        return depositByClientViewModels;

    }
    public async Task<DepositViewModel> GetDepositsAsync()
    {
        var deposits = await _depositRepository.GetAll(DataStatusForRequest.Default);
        var depositByClientViewModels = new DepositViewModel();
        
        depositByClientViewModels.Deposits = new List<DepositByClienViewModel>();
        foreach (var depositByClient in deposits)
        {
            var depositType = await _depositTypeRepository.GetById(depositByClient.TypeId);
            var totalAmountAtEnd = await CalculateTotalAmount(depositByClient.Balance,
                (int)(depositByClient.Time.TotalDays / 30), depositType.Name);
            var depositByClientViewModel = new DepositByClienViewModel()
                { Deposit = depositByClient, DepositType = depositType ,TotalAmountAtEnd = totalAmountAtEnd};
            depositByClientViewModels.Deposits.Add(depositByClientViewModel);
        }
        
        return depositByClientViewModels;

    }
    public async Task<decimal> CalculateTotalAmount(decimal principalAmount, int depositTermInMonths, string depositTypeName)
    {
        var depositType = await _depositTypeRepository.GetByName(depositTypeName);
        if (depositType is null)
        {
            throw new Exception("Тип депозита не найден ");
        }

        decimal annualInterestRate = depositType.InterestRate;
        double depositTermInYears = depositTermInMonths / 12.0;
        decimal totalAmount = principalAmount * (decimal)Math.Pow((double)(1 + annualInterestRate / 100), depositTermInYears);
        return totalAmount;
    }

    public async Task<(decimal,decimal)> CalculateInterestEarned(decimal principalAmount, int depositTermInMonths, string depositTypeName)
    {
        decimal totalAmount =await CalculateTotalAmount(principalAmount, depositTermInMonths, depositTypeName);
        return (totalAmount - principalAmount,totalAmount);
    }

    public async Task<List<Deposit>> Create(string typeName, Guid clientId, TimeSpan time, decimal principalAmount)
    {
        var mounthsCount=(int)(time.TotalDays /30);
        var type = await _depositTypeRepository.GetByName(typeName);
        var interestRateAndTotalAmount = await CalculateInterestEarned(principalAmount, mounthsCount, typeName);
        var deposit = new Deposit()
        {
            Balance = principalAmount,
            ClientId = clientId,
            InterestRate = interestRateAndTotalAmount.Item1,
            IsClosed = false,
            Number = GenerateNumber(),
            Time = time,
            TypeId = type.Id
        };
        await _depositRepository.Create(deposit);
        var deposits = await _depositRepository.GetByClientId(clientId);
        return deposits;
    }
    public async Task<List<Deposit>> UpToDepositBalance(Guid accountId, decimal amount,string note)
    {
        var deposit = await _depositRepository.GetById(accountId);
        var depositType = await _depositTypeRepository.GetById(deposit.TypeId);
        if (deposit is null || deposit.IsClosed)
        {
            throw new Exception("Счет не найден");
        }
        if (depositType.Name!="С возможностью пополнения")
        {
            throw new Exception("Невозможно пополнить счет");
        }
        await _transactionService.CreateDepositUpTransaction(accountId, amount, note);
        deposit.Balance += amount;
        var interestRateAndTotalAmount = await CalculateInterestEarned(deposit.Balance,(int)(deposit.Time.TotalDays /30) , depositType.Name);
        deposit.InterestRate = interestRateAndTotalAmount.Item1;
        await _depositRepository.Update(deposit);
        return await _depositRepository.GetByClientId(deposit.ClientId);

    }
    public async Task<List<Deposit>> WithdrawFromDeposit(Guid accountId, decimal amount,string note)
    {
        var deposit = await _depositRepository.GetById(accountId);
        
        if (deposit is null)
        {
            throw new Exception("Счет не найден");
        }
        if (deposit.Balance<amount)
        {
            await _transactionService.CreateDeposittWithdrawTransaction(accountId, amount, note, isCanceled: true);
            throw new AccountOperationException("Недастаточно денег на счете");
        }
        deposit.Balance -= amount;
        await _depositRepository.Update(deposit);
        
        await _transactionService.CreateDeposittWithdrawTransaction(accountId, amount, note, isCanceled: false);
        
        return await _depositRepository.GetByClientId(deposit.ClientId);
    }
    private string GenerateNumber()
    {
        int length = 10;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var stringBuilder = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            stringBuilder.Append(chars[Random.Shared.Next(chars.Length)]);
        }

        return stringBuilder.ToString();
    }

    public async Task Delete(Guid id)
    {
        var deposit = await _depositRepository.GetById(id);
        if (deposit is null)
        {
            throw new Exception("Депозит не найден ");
        }

        await _depositRepository.Delete(deposit);
    }

    public async Task<DepositType> GetTypeByDepositId(Guid depositId)
    {
        var deposit = await _depositRepository.GetById(depositId);
        return await _depositTypeRepository.GetById(deposit.TypeId);
    }

    public async Task<List<DepositType>> GetDepositTypes()
    {
        return await _depositTypeRepository.GetAll(DataStatusForRequest.Default);
    }
    
}