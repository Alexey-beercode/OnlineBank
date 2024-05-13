using Microsoft.EntityFrameworkCore;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.DataManagment.Repositories.Interfaces;

namespace OnlineBank.DataManagment.Repositories.Implementations;

public class TransactionRepository:IBaseRepository<Transaction>
{
    private readonly ApplicationDbContext _dbContext;

    public TransactionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Transaction> GetById(Guid id)
    {
        return _dbContext.Transactions.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Transaction> GetByNumber(string number)
    {
        return await _dbContext.Transactions.FirstOrDefaultAsync(a => a.Number == number);
    }
    public async Task Update(Transaction entity)
    {
        _dbContext.Transactions.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Transaction entity)
    {
        entity.IsCanceled = true;
        _dbContext.Transactions.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Transaction>> GetAll(DataStatusForRequest dataStatusForRequest)
    {
        switch (dataStatusForRequest)
        {
            case DataStatusForRequest.Active:
                return await _dbContext.Transactions.Where(a => a.IsCanceled == false).ToListAsync();
            case DataStatusForRequest.Deleted:
                return await _dbContext.Transactions.Where(a => a.IsCanceled == true).ToListAsync();
        }

        return await _dbContext.Transactions.ToListAsync();
    }

    public async Task<List<Transaction>> GetByDepositId(Guid depositId)
    {
        return await _dbContext.Transactions.Where(a => a.DepositId == depositId).ToListAsync();
    }

    public async Task Create(Transaction transaction)
    {
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Transaction>> GetByTypeName(string typeName)
    {
        var type = await _dbContext.TransactionTypes.FirstOrDefaultAsync(a => a.Name == typeName);
        return await _dbContext.Transactions.Where(a => a.TypeId == type.Id).ToListAsync();
    }

    public async Task<List<Transaction>> GetByTypeId(Guid typeId)
    {
        return await _dbContext.Transactions.Where(a => a.TypeId == typeId).ToListAsync();
    }
}