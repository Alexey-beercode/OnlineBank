using Microsoft.AspNetCore.Mvc;
using OnlineBank.Data.ViewModels;
using OnlineBank.Service.Service;
using OnlineBank.Service.Services;

namespace OnlineBank.Controllers;

public class AccountController : Controller
{
    private readonly ClientService _clientService;
    private readonly UserService _userService;
    private readonly AccountService _accountService;

    public AccountController(ClientService clientService, UserService userService, AccountService accountService)
    {
        _clientService = clientService;
        _userService = userService;
        _accountService = accountService;
    }

    public IActionResult GetByUser()
    {
        return View();
    }
    private async Task<Guid> GetClientId()
    {
        string userIdString = User.FindFirst("UserId")?.Value;
        var guidId = Guid.Parse(userIdString);
        var user = await _userService.GetByIdAsync(guidId);
        return user.ClientId;
    }
    [HttpGet]
    public async Task<IActionResult> UpToAccount(Guid accountId)
    {
        return View(new AccountOperationViewModel(){AccountId = accountId});
    }
    [HttpGet]
    public async Task<IActionResult> WithdrawFromAccount(Guid accountId)
    {
        return View(new AccountOperationViewModel(){AccountId = accountId});
    }

    [HttpPost]
    public async Task<IActionResult> UpToAccount(AccountOperationViewModel operationViewModel)
    {
        var clientId = await GetClientId();
        var client = await _clientService.GetByIdAsync(clientId.ToString());
        var accounts = await _accountService.UpToAccountBalance(operationViewModel.AccountId, operationViewModel.Amount,
            operationViewModel.Note);
        return View("GetByUser",
            new AccountsViewModel() { Accounts = accounts, ClientSurname = client.Surname, CLientName = client.Name });
    }

    [HttpPost]
    public async Task<IActionResult> WithdrawFromAccount(AccountOperationViewModel operationViewModel)
    {
        var clientId = await GetClientId();
        var client = await _clientService.GetByIdAsync(clientId.ToString());
        var accounts = await _accountService.WithdrawFromAccount(operationViewModel.AccountId, operationViewModel.Amount,
            operationViewModel.Note);
        return View("GetByUser",
            new AccountsViewModel() { Accounts = accounts, ClientSurname = client.Surname, CLientName = client.Name });
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
}