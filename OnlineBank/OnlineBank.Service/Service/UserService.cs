using System.Security.Claims;
using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.Data.ViewModel;
using OnlineBank.DataManagment.Repositories.Implementations;
using OnlineBank.DataManagment.Repositories.Interfaces;

namespace OnlineBank.Service.Service;

public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly RoleRepository _roleRepository;

    private int _workFactor = 12;
    
    public UserService(UserRepository userRepository, RoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
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
        string salt = BCrypt.Net.BCrypt.GenerateSalt(_workFactor);
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash, salt);
        
        var userFromDb =await _userRepository.GetById(user.Id);
        if (userFromDb==null)
        {
            throw new Exception("Пользователь не найден");
        }
        userFromDb.Login = user.Login;
        userFromDb.ClientId = user.ClientId;
        userFromDb.PasswordHash = hashedPassword;
        await _userRepository.Update(userFromDb);
        return await _userRepository.GetAll(DataStatusForRequest.Default);
    }

    public async Task<List<User>> Create(User user)
    {
        string salt = BCrypt.Net.BCrypt.GenerateSalt(_workFactor);
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash, salt);

        user.PasswordHash = hashedPassword;
        
        await _userRepository.Create(user);
        
        //TODO: Не знаю специально ли ты это упустил или нет, но я пожалуй добавлю
        await _roleRepository.SetRoleToUser("Resident", user.Id);
        
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
    
    public async Task<ClaimsIdentity> LoginAsync(LoginViewModel model)
    {
        var user = await _userRepository.GetByLogin(model.Login);

        if(user is null)
        {
            throw new Exception("Логин или пароль введены некорректно.");
        }
        
        if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            throw new Exception("Логин или пароль введены некорректно.");
        }

        return await Authenticate(user);
    }
    
    public async Task<ClaimsIdentity> RegisterAsync(RegisterViewModel model)
    {
        var user = await _userRepository.GetByLogin(model.Login);
        if (user is not null)
        {
            throw new Exception("Логин уже используется.");
        }

        string salt = BCrypt.Net.BCrypt.GenerateSalt(_workFactor);
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password, salt);
        
        user = new User()
        {
            Login = model.Login,
            PasswordHash = hashedPassword,
            IsDeleted = false
        };

        await _userRepository.Create(user);

        await _roleRepository.SetRoleToUser("Resident", user.Id);
        
        return await Authenticate(user);
    }
    
    private async Task<ClaimsIdentity> Authenticate(User user)
    {   
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
            new Claim("UserId", user.Id.ToString()),
        };

        return new ClaimsIdentity(claims, "Authentication", ClaimsIdentity.DefaultNameClaimType, null);
    }
}