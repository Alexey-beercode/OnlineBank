using System.Text;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.Data.ViewModels;
using OnlineBank.DataManagment.Repositories.Implementations;
using OnlineBank.Service.Exceptions;
using OnlineBank.Service.Service;

namespace OnlineBank.Service.Services;

public class AccountService
{
    private readonly AccountRepository _accountRepository;
    private readonly TransactionService _transactionService;
    private readonly ClientService _clientService;

    public AccountService(AccountRepository accountRepository, TransactionService transactionService, ClientService clientService)
    {
        _accountRepository = accountRepository;
        _transactionService = transactionService;
        _clientService = clientService;
    }

    public async Task<Account> GetById(Guid id)
    {
        var account = await _accountRepository.GetById(id);
        if (account is null)
        {
            throw new Exception("Счет не найден");
        }

        return account;
    }

    public async Task<Account> GetByNumber(string number)
    {
        var account = await _accountRepository.GetByNumber(number);
        if (account is null)
        {
            throw new Exception("Счет не найден");
        }

        return account;
    }

    public async Task<List<Account>> UpToAccountBalance(Guid accountId, decimal amount,string note)
    {
        var account = await _accountRepository.GetById(accountId);
        if (account is null || account.IsClosed)
        {
            throw new Exception("Счет не найден");
        }
        await _transactionService.CreateAcoountUpTransaction(accountId, amount, note);
        account.Balance += amount;
        await _accountRepository.Update(account);
        var accounts = await _accountRepository.GetByCLientId(account.ClientId);
        return accounts;

    }
    public async Task<List<Account>> WithdrawFromAccount(Guid accountId, decimal amount,string note)
    {
        var account = await _accountRepository.GetById(accountId);
        if (account is null)
        {
            throw new Exception("Счет не найден");
        }
        if (account.Balance < amount)
        {
            await _transactionService.CreateAcoountWithdrawTransaction(accountId, amount, note, isCanceled: true);
            throw new AccountOperationException("Недастаточно денег на счете");
        }

        account.Balance -= amount;
        await _accountRepository.Update(account);
        
        await _transactionService.CreateAcoountWithdrawTransaction(accountId, amount, note, isCanceled: false);
        
        var accounts = await _accountRepository.GetByCLientId(account.ClientId);
        return accounts;
    }

    public async Task<List<Account>> GetAll()
    {
        var accounts = await _accountRepository.GetAll(DataStatusForRequest.Default);
        if (accounts.Count == 0)
        {
            throw new Exception("Не найдено счетов");
        }

        return accounts;
    }
    
    public async Task<List<AccountWithClientViewModel>> GetAllWithClient()
    {
        var accountsWithClient = new List<AccountWithClientViewModel>();
        
        var accounts = await _accountRepository.GetAll(DataStatusForRequest.Default);
        foreach (var account in accounts)
        {
            var client = await _clientService.GetByIdAsync(account.ClientId.ToString());
            
            accountsWithClient.Add(new AccountWithClientViewModel(){Account = account, Client = client});
        }
        
        return accountsWithClient;
    }

    public async Task<List<Account>> GetByClientId(Guid clientId)
    {
        var accountsByUser = await _accountRepository.GetByCLientId(clientId);
        return accountsByUser;
    }

    public async Task CloseAccount(Guid accountId, Guid reserveAccountId=default)
    {
        var account = await _accountRepository.GetById(accountId);
        if (account is null || account.IsClosed)
        {
            throw new Exception("Счет не найден");
        }

        if (reserveAccountId==default)
        {
            await _accountRepository.Delete(account);
            return;
        }

        var reserveAccount = await _accountRepository.GetById(reserveAccountId);
        if (reserveAccount is null || reserveAccount.IsClosed)
        {
            throw new Exception("Счет не найден");
        }

        reserveAccount.Balance += account.Balance;
        await _accountRepository.Update(reserveAccount);
    }

    public async Task<List<Account>> Create(Guid clientId)
    {
        var account = new Account()
        {
            Balance = 0,
            ClientId = clientId,
            IsClosed = false,
            Number = GenerateNumber()
        };
        await _accountRepository.Create(account);
        return await _accountRepository.GetByCLientId(clientId);
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
        var account = await _accountRepository.GetById(id);
        if (account is null)
        {
            throw new Exception("Депозит не найден ");
        }

        await _accountRepository.Delete(account);
    }
}