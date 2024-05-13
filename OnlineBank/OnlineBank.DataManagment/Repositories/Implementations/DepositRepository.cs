using Microsoft.EntityFrameworkCore;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.DataManagment.Repositories.Interfaces;

namespace OnlineBank.DataManagment.Repositories.Implementations;

public class DepositRepository:IBaseRepository<Deposit>
{
    private readonly ApplicationDbContext _dbContext;

    public DepositRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Deposit> GetById(Guid id)
    {
        return await _dbContext.Deposits.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task Update(Deposit entity)
    {
        _dbContext.Deposits.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Deposit entity)
    {
        entity.IsClosed = true;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Deposit>> GetAll(DataStatusForRequest dataStatusForRequest)
    {
        switch (dataStatusForRequest)
        {
            case DataStatusForRequest.Active:
                return await _dbContext.Deposits.Where(a => a.IsClosed == false).ToListAsync();
            case DataStatusForRequest.Deleted:
                return await _dbContext.Deposits.Where(a => a.IsClosed == true).ToListAsync();
            default:
                return await _dbContext.Deposits.ToListAsync();
        }
    }

    public async Task Create(Deposit entity)
    {
        _dbContext.Deposits.Add(entity);
       await _dbContext.SaveChangesAsync();
    }
}