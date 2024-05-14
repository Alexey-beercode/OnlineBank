using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.DataManagment.Repositories.Implementations;

namespace OnlineBank.Service.Service;

public class RoleService
{
    private readonly RoleRepository _roleRepository;

    public RoleService(RoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<List<Role>> GetAll()
    {
        return await _roleRepository.GetAll(DataStatusForRequest.Default);
    }

    public async Task<List<User>> GetUsersByRoleId(Guid roleId)
    {
        return await _roleRepository.GetUsersByRoleId(roleId);
    }

    public async Task Delete(Guid roleId)
    {
        var role = await _roleRepository.GetById(roleId);
        if (role==null)
        {
            throw new Exception("Роль не найдена");
        }
        await _roleRepository.Delete(role);
    }

    public async Task DeleteRoleFromUser(string roleName, Guid userId)
    {
        var role = await _roleRepository.GetByName(roleName);
        if (role==null)
        {
            throw new Exception("роль не найдена");
        }

        var isRoleHasUser = await _roleRepository.CheckIsRoleHasUser(role.Id, userId);
        if (!isRoleHasUser)
        {
            throw new Exception("Не найдено такой роли у пользователя");
        }
        await _roleRepository.DeleteRoleFromUser(roleName, userId);
    }

    public async Task SetRoleToUser(string roleName, Guid userId)
    {
        var role = await _roleRepository.GetByName(roleName);
        if (role==null)
        {
            throw new Exception("роль не найдена");
        }

        var isRoleHasUser = await _roleRepository.CheckIsRoleHasUser(role.Id, userId);
        if (!isRoleHasUser)
        {
            throw new Exception("Не найдено такой роли у пользователя");
        }
        await _roleRepository.SetRoleToUser(roleName, userId);
    }
}