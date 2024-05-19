using Microsoft.EntityFrameworkCore;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.DataManagment.Repositories.Interfaces;

namespace OnlineBank.DataManagment.Repositories.Implementations;

public class AccountRepository:IBaseRepository<Account>
{
    private readonly ApplicationDbContext _dbContext;

    public AccountRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Account> GetById(Guid id)
    {
        return await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task Update(Account entity)
    {
        _dbContext.Accounts.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Account entity)
    {
        entity.IsClosed = true;
        _dbContext.Accounts.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Account>> GetAll(DataStatusForRequest dataStatusForRequest)
    {
        switch (dataStatusForRequest)
        {
            case DataStatusForRequest.Active:
                return await _dbContext.Accounts.Where(a => a.IsClosed == false).ToListAsync();
            case DataStatusForRequest.Deleted:
                return await _dbContext.Accounts.Where(a => a.IsClosed == false).ToListAsync();
        }
        return await _dbContext.Accounts.ToListAsync();
    }

    public async Task Create(Account entity)
    {
        _dbContext.Accounts.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Account> GetByNumber(string number)
    {
        return await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == number);
    }

    public async Task<List<Account>> GetByCLientId(Guid clientId)
    {
        return await _dbContext.Accounts.Where(a => a.ClientId == clientId && a.IsClosed == false).ToListAsync();
    }
    
}