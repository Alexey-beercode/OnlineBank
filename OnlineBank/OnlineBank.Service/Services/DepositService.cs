using System.Text;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.DataManagment.Repositories.Implementations;

namespace OnlineBank.Service.Services;

public class DepositService
{
    private readonly DepositRepository _depositRepository;
    private readonly DepositTypeRepository _depositTypeRepository;

    public DepositService(DepositRepository depositRepository, DepositTypeRepository depositTypeRepository)
    {
        _depositRepository = depositRepository;
        _depositTypeRepository = depositTypeRepository;
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