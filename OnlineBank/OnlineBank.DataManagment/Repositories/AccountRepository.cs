using Microsoft.EntityFrameworkCore;
using OnlineBank.Data.Entity;
using OnlineBank.DataManagment.Repositories.Interfaces;

namespace OnlineBank.DataManagment.Repositories;

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

    public async Task<List<Account>> GetAll()
    {
        return await _dbContext.Accounts.ToListAsync();
    }

    public async Task Create(Account entity)
    {
        _dbContext.Accounts.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Account>> GetActive()
    {
        return await _dbContext.Accounts.Where(a => a.IsClosed == false).ToListAsync();
    }
}