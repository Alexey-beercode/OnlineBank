using Microsoft.EntityFrameworkCore;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.DataManagment.Repositories.Interfaces;

namespace OnlineBank.DataManagment.Repositories.Implementations;

public class DepositTypeRepository:IBaseRepository<DepositType>
{
    private readonly ApplicationDbContext _dbContext;

    public DepositTypeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DepositType> GetById(Guid id)
    {
        return await _dbContext.DepositTypes.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task Update(DepositType entity)
    {
        _dbContext.DepositTypes.Update(entity);
       await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(DepositType entity)
    {
        _dbContext.DepositTypes.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<DepositType>> GetAll(DataStatusForRequest dataStatusForRequest)
    {
        return await _dbContext.DepositTypes.ToListAsync();
    }

    public async Task Create(DepositType entity)
    {
        _dbContext.DepositTypes.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<DepositType> GetByName(string name)
    {
        return await _dbContext.DepositTypes.FirstOrDefaultAsync(a => a.Name==name);
    }
}