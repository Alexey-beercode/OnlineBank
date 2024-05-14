using Microsoft.EntityFrameworkCore;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.DataManagment.Repositories.Interfaces;

namespace OnlineBank.DataManagment.Repositories.Implementations;

public class RoleRepository:IBaseRepository<Role>
{
    private readonly ApplicationDbContext _dbContext;

    public RoleRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Role>> GetAll(DataStatusForRequest dataStatusForRequest)
    {
        return await _dbContext.Roles.ToListAsync();
    }

    public async Task Create(Role entity)
    {
        _dbContext.Roles.Add(entity);
       await _dbContext.SaveChangesAsync();
    }

    public async Task<Role> GetById(Guid id)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task Update(Role entity)
    {
        _dbContext.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(Role entity)
    {
        _dbContext.Roles.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<User>> GetUsersByRoleId(Guid roleId)
    {
        var userIds = await _dbContext.UsersRoles.Where(a => a.RoleId == roleId).Select(a=>a.Userid).ToListAsync();
        var users = await _dbContext.Users.Where(a => userIds.Contains(a.Id)).ToListAsync();
        return users;
    }

    public async Task SetRoleToUser(string roleName, Guid userId)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(a => a.Name == roleName);
        _dbContext.UsersRoles.Add(new UserRole() { IsDeleted = false, RoleId = role.Id, Userid = userId });
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteRoleFromUser(string roleName, Guid userId)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(a => a.Name == roleName);
        var userRole = await _dbContext.UsersRoles.FirstOrDefaultAsync(a => a.Userid == userId && a.RoleId == role.Id);
        _dbContext.UsersRoles.Remove(userRole);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Role> GetByName(string name)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(a => a.Name==name);
    }

    public async Task<bool> CheckIsRoleHasUser(Guid roleId, Guid userId)
    {
        var userRole = await _dbContext.UsersRoles
            .FirstOrDefaultAsync(a => a.RoleId == roleId && a.Userid == userId && a.IsDeleted == false);
        if (userRole!=null)
        {
            return true;
        }

        return false;
    }
}