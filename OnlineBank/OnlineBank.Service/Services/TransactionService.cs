using System.Text;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.Data.ViewModels;
using OnlineBank.DataManagment.Repositories.Implementations;
using OnlineBank.Service.Service;

namespace OnlineBank.Service.Services;

public class TransactionService
{
    private readonly TransactionRepository _transactionRepository;
    private readonly TransactionTypeRepository _transactionTypeRepository;
    private readonly DepositRepository _depositRepository;
    private readonly AccountRepository _accountRepository;
    private readonly ClientService _clientService;

    public TransactionService(TransactionTypeRepository transactionTypeRepository, TransactionRepository transactionRepository, DepositRepository depositRepository, AccountRepository accountRepository, ClientService clientService)
    {
        _transactionTypeRepository = transactionTypeRepository;
        _transactionRepository = transactionRepository;
        _depositRepository = depositRepository;
        _accountRepository = accountRepository;
        _clientService = clientService;
    }

    public async Task CreateAcoountUpTransaction(Guid accountId,decimal amount, string note)
    {
        var type = await _transactionTypeRepository.GetByName("Пополнение");
        var transaction = new Transaction()
        {
            AccountId = accountId,
            Amount = amount,
            Date = DateTime.Now,
            IsCanceled = false,
            Note = note,
            Number = GenerateTransactionNumber(),
            TypeId = type.Id
        };
        await _transactionRepository.Create(transaction);
    }

    public async Task CreateAcoountWithdrawTransaction(Guid accountId, decimal amount, string note, bool isCanceled)
    {
        var type = await _transactionTypeRepository.GetByName("Снятие");
        var transaction = new Transaction()
        {
            AccountId = accountId,
            Amount = amount,
            Date = DateTime.Now,
            IsCanceled = isCanceled,
            Note = note,
            Number = GenerateTransactionNumber(),
            TypeId = type.Id
        };
        await _transactionRepository.Create(transaction);
    }
    public async Task CreateDepositUpTransaction(Guid depositId,decimal amount, string note)
    {
        var type = await _transactionTypeRepository.GetByName("Пополнение");
        var transaction = new Transaction()
        {
            DepositId = depositId,
            Amount = amount,
            Date = DateTime.Now,
            IsCanceled = false,
            Note = note,
            Number = GenerateTransactionNumber(),
            TypeId = type.Id
        };
        await _transactionRepository.Create(transaction);
    }

    public async Task CreateDeposittWithdrawTransaction(Guid depositId, decimal amount, string note, bool isCanceled)
    {
        var type = await _transactionTypeRepository.GetByName("Снятие");
        var transaction = new Transaction()
        {
            DepositId = depositId,
            Amount = amount,
            Date = DateTime.Now,
            IsCanceled = isCanceled,
            Note = note,
            Number = GenerateTransactionNumber(),
            TypeId = type.Id
        };
        await _transactionRepository.Create(transaction);
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
    public  string GenerateTransactionNumber()
    {
        var currentDateTime = DateTime.Now;
        var datePart = currentDateTime.ToString("yyyyMMddHHmmss");
        var randomPart = GenerateNumber();

        return $"{datePart}-{randomPart}";
    }

    public async Task<TransactionsViewModel> GetByClienIdAsync(Guid clientId)
    {
        var client = await _clientService.GetByIdAsync(clientId.ToString());
        var depositsByCLient = await _depositRepository.GetByClientId(clientId);
        var accountsByClient = await _accountRepository.GetByCLientId(clientId);
        var transactionsByUser = new List<Transaction>();
        var transactionViewModels = new List<TransactionViewModel>();
        foreach (var deposit in depositsByCLient)
        {
            var transactionsByDeposit = await _transactionRepository.GetByDepositId(deposit.Id);
            transactionsByUser.AddRange(transactionsByDeposit);
        }

        foreach (var account in accountsByClient)
        {
            var transactionsByAccount = await _transactionRepository.GetByAccountId(account.Id);
            transactionsByUser.AddRange(transactionsByAccount);
        }

        foreach (var transaction in transactionsByUser)
        {
            var transactionType = await _transactionTypeRepository.GetById(transaction.TypeId);
            var depositNumber = "";
            if (transaction.DepositId!=default)
            {
                 depositNumber= (await _depositRepository.GetById(transaction.DepositId)).Number;
            }
            else
            {
                depositNumber = (await _accountRepository.GetById(transaction.AccountId)).Number;
            }
            transactionViewModels.Add(new TransactionViewModel(){DepositNumber = depositNumber,Transaction = transaction,TransactionType = transactionType});
            
        }

        return new TransactionsViewModel()
            { ClientSurname = client.Surname, ClientName = client.Name, TransactionViewModels = transactionViewModels };
    }

    public async Task<TransactionsViewModel> GetByDepositOrAccount(Guid clientId,Guid depositId = default, Guid accountId = default)
    {
        var client = await _clientService.GetByIdAsync(clientId.ToString());
        var transactions = new List<Transaction>();
        var transactionViewModels = new List<TransactionViewModel>();
        if (accountId!=default)
        {
            transactions = await _transactionRepository.GetByAccountId(accountId);
        }
        else
        {
            transactions = await _transactionRepository.GetByDepositId(depositId);
        }
        foreach (var transaction in transactions)
        {
            var transactionType = await _transactionTypeRepository.GetById(transaction.TypeId);
            var depositNumber = "";
            if (transaction.DepositId!=default)
            {
                depositNumber= (await _depositRepository.GetById(transaction.DepositId)).Number;
            }
            else
            {
                depositNumber = (await _accountRepository.GetById(transaction.AccountId)).Number;
            }
            transactionViewModels.Add(new TransactionViewModel(){DepositNumber = depositNumber,Transaction = transaction,TransactionType = transactionType});
        }
        return new TransactionsViewModel()
            { ClientSurname = client.Surname, ClientName = client.Name, TransactionViewModels = transactionViewModels };
    }

    public async Task<List<TransactionViewModel>> GetAll()
    {
        var transactions = await _transactionRepository.GetAll(DataStatusForRequest.Active);
        var transactionViewModels = new List<TransactionViewModel>();
        
        foreach (var transaction in transactions)
        {
            var transactionType = await _transactionTypeRepository.GetById(transaction.TypeId);
            var depositNumber = "";
            if (transaction.DepositId != default)
            {
                var deposit = await _depositRepository.GetById(transaction.DepositId);
                if (deposit is null)
                {
                    continue;
                }
                depositNumber= deposit.Number;
            }
            else
            {
                var account = await _accountRepository.GetById(transaction.AccountId);
                if (account is null)
                {
                    continue;
                }
                
                depositNumber = account.Number;
            }
            transactionViewModels.Add(new TransactionViewModel(){DepositNumber = depositNumber,Transaction = transaction,TransactionType = transactionType});
        }

        return transactionViewModels;
    }
}