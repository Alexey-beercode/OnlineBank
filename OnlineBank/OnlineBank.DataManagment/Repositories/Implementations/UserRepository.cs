using Microsoft.EntityFrameworkCore;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.DataManagment.Repositories.Interfaces;

namespace OnlineBank.DataManagment.Repositories.Implementations;

public class UserRepository:IBaseRepository<User>
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Create(User entity)
    {
        _dbContext.Users.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> GetById(Guid id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<User>> GetAll(DataStatusForRequest dataStatusForRequest)
    {
        switch (dataStatusForRequest)
        {
            case DataStatusForRequest.Active:
                return await _dbContext.Users.Where(a => a.IsDeleted == false).ToListAsync();
            case DataStatusForRequest.Deleted:
                return await _dbContext.Users.Where(a => a.IsDeleted == true).ToListAsync();
        }
        return await _dbContext.Users.ToListAsync();
    }
    

    public async Task Delete(User entity)
    {
        entity.IsDeleted = true;
        _dbContext.Users.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Role>> GetRolesByUser(Guid id)
    {
        var rolesIds=await _dbContext.UsersRoles.Where(a => a.Userid == id).Select(a => a.RoleId).ToListAsync();
        var roles = await _dbContext.Roles
            .Where(r => rolesIds.Contains(r.Id))
            .ToListAsync();
        return roles;
    }

    public async Task Update(User entity)
    {
        _dbContext.Users.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> GetByLogin(string login)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(a => a.Login == login);
    }
}