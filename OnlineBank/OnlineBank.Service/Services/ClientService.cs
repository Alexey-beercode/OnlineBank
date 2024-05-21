using OnlineBank.Data.Entity;
using OnlineBank.Data.Enum;
using OnlineBank.Data.ViewModel;
using OnlineBank.DataManagment.Repositories.Implementations;
using OnlineBank.Service.Mapper;

namespace OnlineBank.Service.Service;

public class ClientService
{
    private readonly ClientRepository _clientRepository;
    private readonly UserRepository _userRepository;

    public ClientService(ClientRepository clientRepository, UserRepository userRepository)
    {
        _clientRepository = clientRepository;
        _userRepository = userRepository;
    }

    public async Task<Client> GetByIdAsync(string userId)
    {
        var client = await _clientRepository.GetById(StringToGuidMapper.MapTo(userId));
        if (client == null)
        {
            throw new Exception("Данные пользователя не найдены.");
        }

        return client;
    }

    public async Task<Client> CreateClientAsync(ClientViewModel clientViewModel, string userId)
    {
        var user = await _userRepository.GetById(StringToGuidMapper.MapTo(userId));
        if (user == null)
        {
            throw new Exception("Пользователь не найден.");
        }

        var client = new Client()
        {
            Address = clientViewModel.Address,
            BirthDay = clientViewModel.BirthDay,
            Name = clientViewModel.Name,
            Surname = clientViewModel.Surname,
            Phone = clientViewModel.Phone,
            IsDeleted = false
        };

        await _clientRepository.Create(client);

        user.ClientId = client.Id;
        await _userRepository.Update(user);

        return client;
    }
    
    public async Task DeleteClientAsync(string userId)
    {
        var user = await _userRepository.GetById(StringToGuidMapper.MapTo(userId));
        if (user == null)
        {
            throw new Exception("Пользователь не найден.");
        }

        var client = await _clientRepository.GetById(user.ClientId);
        if (client is null)
        {
            throw new Exception("Данные клиента не найдены.");
        }

        await _clientRepository.Delete(client);

        user.ClientId = Guid.Empty;
        await _userRepository.Update(user);
    }

    public async Task<List<Client>> GetAll()
    {
        var clients = await _clientRepository.GetAll(DataStatusForRequest.Default);
        return clients;
    }

    public async Task<Client> GetByNameAndSurname(string name, string surname)
    {
        var client =
            (await _clientRepository.GetAll(DataStatusForRequest.Active)).FirstOrDefault(a =>
                a.Name == name && a.Surname == surname);
        if (client is null)
        {
            throw new Exception("Клиент не найден ");
        }

        return client;
    }

    public async Task<List<Client>> Update(Client client)
    {
        var clientFromDatabase = await _clientRepository.GetById(client.Id);
        if (clientFromDatabase is null)
        {
            throw new Exception("Клиент не найден");
        }

        await _clientRepository.Update(client);
        return await _clientRepository.GetAll(DataStatusForRequest.Default);
    }
}