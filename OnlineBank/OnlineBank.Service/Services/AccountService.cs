using System.Text;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.DataManagment.Repositories.Implementations;
using OnlineBank.Service.Exceptions;

namespace OnlineBank.Service.Service;

public class AccountService
{
    private readonly AccountRepository _accountRepository;


    public AccountService(AccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
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

    public async Task<Account> UpToAccountBalance(Guid accountId, decimal amount,string note)
    {
        var account = await _accountRepository.GetById(accountId);
        if (account is null || account.IsClosed)
        {
            throw new Exception("Счет не найден");
        }
        
        account.Balance += amount;
        await _accountRepository.Update(account);
        return account;

    }
    public async Task<Account> WithdrawFromAccount(Guid accountId, decimal amount)
    {
        var account = await _accountRepository.GetById(accountId);
        if (account is null)
        {
            throw new Exception("Счет не найден");
        }
        if (account.Balance<amount)
        {
            throw new AccountOperationException("Недастаточно денег на счете");
        }

        account.Balance -= amount;
        await _accountRepository.Update(account);
        return account;
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

    public async Task<List<Account>> GetByClientId(Guid clientId)
    {
        var accountsByUser = await _accountRepository.GetByCLientId(clientId);
        if (accountsByUser.Count==0)
        {
            throw new Exception("Не найдено счетов");
        }

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
}