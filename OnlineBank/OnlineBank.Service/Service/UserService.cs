using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.DataManagment.Repositories.Implementations;
using OnlineBank.DataManagment.Repositories.Interfaces;

namespace OnlineBank.Service.Service;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<User>> GetAll()
    {
        return await _userRepository.GetAll(DataStatusForRequest.Default);
    }

    public async Task<User> GetByLogin(string login)
    {
        var user = await _userRepository.GetByLogin(login);
        if (user==null)
        {
            throw new Exception("Пользователь не найден");
        }
        return user;
    }

    public async Task<List<User>> Delete(Guid userId)
    {
        var user =await _userRepository.GetById(userId);
        if (user==null)
        {
            throw new Exception("Пользователь не найден");
        }
        await _userRepository.Delete(user);
        return await _userRepository.GetAll(DataStatusForRequest.Default);
    }

    public async Task<List<User>> GetActive()
    {
        return await _userRepository.GetAll(DataStatusForRequest.Active);
    }

    public async Task<List<User>> Update(User user)
    {
        var userFromDb =await _userRepository.GetById(user.Id);
        if (userFromDb==null)
        {
            throw new Exception("Пользователь не найден");
        }
        userFromDb.Login = user.Login;
        userFromDb.ClientId = user.ClientId;
        userFromDb.PasswordHash = user.PasswordHash;
        await _userRepository.Update(userFromDb);
        return await _userRepository.GetAll(DataStatusForRequest.Default);
    }

    public async Task<List<User>> Create(User user)
    {
        await _userRepository.Create(user);
        return await _userRepository.GetAll(DataStatusForRequest.Default);
    }

    public async Task<List<Role>> GetRolesByUser(Guid userId)
    {
        var roles = await _userRepository.GetRolesByUser(userId);
        return roles;
    }

    public async Task<bool> CheckUserHasRole(Guid userId, string roleName)
    {
        var roles = await _userRepository.GetRolesByUser(userId);
        if (roles.Select(a=>a.Name).Contains(roleName))
        {
            return true;
        }

        return false;
    }
}