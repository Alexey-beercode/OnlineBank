using System.Text;
using OnlineBank.Data.Entity;
using OnlineBank.DataManagment.Repositories.Implementations;

namespace OnlineBank.Service.Service;

public class TransactionService
{
    private readonly TransactionRepository _transactionRepository;
    private readonly TransactionTypeRepository _transactionTypeRepository;

    public TransactionService(TransactionTypeRepository transactionTypeRepository, TransactionRepository transactionRepository)
    {
        _transactionTypeRepository = transactionTypeRepository;
        _transactionRepository = transactionRepository;
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
}