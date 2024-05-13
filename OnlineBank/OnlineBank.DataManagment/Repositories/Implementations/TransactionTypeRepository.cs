using Microsoft.EntityFrameworkCore;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.DataManagment.Repositories.Interfaces;

namespace OnlineBank.DataManagment.Repositories.Implementations;

public class TransactionTypeRepository:IBaseRepository<TransactionType>
{
    private readonly ApplicationDbContext _dbContext;

    public TransactionTypeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TransactionType> GetById(Guid id)
    {
        return await _dbContext.TransactionTypes.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task Update(TransactionType entity)
    {
        _dbContext.TransactionTypes.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(TransactionType entity)
    {
        _dbContext.TransactionTypes.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<TransactionType>> GetAll(DataStatusForRequest dataStatusForRequest)
    {
        return await _dbContext.TransactionTypes.ToListAsync();
    }

    public async Task Create(TransactionType entity)
    {
        _dbContext.TransactionTypes.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<TransactionType> GetByName(string name)
    {
        return await _dbContext.TransactionTypes.FirstOrDefaultAsync(a => a.Name == name);
    }
}