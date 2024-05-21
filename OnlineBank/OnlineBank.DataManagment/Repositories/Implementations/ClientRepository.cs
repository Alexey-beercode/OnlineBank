using Microsoft.EntityFrameworkCore;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.DataManagment.Repositories.Interfaces;

namespace OnlineBank.DataManagment.Repositories.Implementations;

public class ClientRepository:IBaseRepository<Client>
{
    private readonly ApplicationDbContext _dbContext;

    public ClientRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Client> GetById(Guid id)
    {
        return await _dbContext.Clients.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task Update(Client entity)
    {
        _dbContext.Clients.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Client entity)
    {
        entity.IsDeleted = true;
        _dbContext.Clients.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Client>> GetAll(DataStatusForRequest dataStatusForRequest)
    {
        switch (dataStatusForRequest)
        {
            case DataStatusForRequest.Active:
                return await _dbContext.Clients.Where(a => a.IsDeleted == false).ToListAsync();
            case DataStatusForRequest.Deleted:
                return await _dbContext.Clients.Where(a => a.IsDeleted == true).ToListAsync();
        }

        return await _dbContext.Clients.ToListAsync();
    }

    public async Task Create(Client entity)
    {
        _dbContext.Clients.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Client>> SearchByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new List<Client>();
        }

        // Разделение полного имени на части с удалением пустых элементов
        var nameParts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        // Формирование запроса с учетом количества частей
        IQueryable<Client> query = _dbContext.Clients;

        if (nameParts.Length == 1)
        {
            string part = nameParts[0];
            query = query.Where(c => c.Name.StartsWith(part) || c.Surname.StartsWith(part));
        }
        else if (nameParts.Length >= 2)
        {
            string firstName = nameParts[0];
            string lastName = nameParts[1];
            query = query.Where(c => (c.Name.StartsWith(firstName) && c.Surname.StartsWith(lastName)) ||
                                     (c.Name.StartsWith(lastName) && c.Surname.StartsWith(firstName)));
        }

        return await query.ToListAsync();
    }
}