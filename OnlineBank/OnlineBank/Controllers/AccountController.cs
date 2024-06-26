﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBank.Data.ViewModels;
using OnlineBank.Service.Service;
using OnlineBank.Service.Services;

namespace OnlineBank.Controllers;

[Authorize]
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

    public async Task<IActionResult> GetByUser()
    {
        try
        {
            var accountViewModel = new AccountsViewModel();
            
            var id = await GetClientId();
            if (id.Equals(Guid.Empty))
            {
                return Redirect($"/User/CreateClient/");
            }
            
            var accountsByUser = await _accountService.GetByClientId(id);
            var client = await _clientService.GetByIdAsync(id.ToString());
            
            accountViewModel.Accounts = accountsByUser;
            accountViewModel.CLientName = client.Name;
            accountViewModel.ClientSurname = client.Surname;   
                
            return View(accountViewModel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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
        var account = await _accountService.GetById(accountId);
        
        return View(new AccountOperationViewModel(){AccountId = accountId, Balance = account.Balance});
    }
    [HttpGet]
    public async Task<IActionResult> WithdrawFromAccount(Guid accountId)
    {
        var account = await _accountService.GetById(accountId);
        
        return View(new AccountOperationViewModel(){AccountId = accountId, Balance = account.Balance});
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
    public async Task<IActionResult> Create()
    {
        var id = await GetClientId();
        if (id.Equals(Guid.Empty))
        {
            return Redirect($"/User/CreateClient/");
        }
        
        await _accountService.Create(id);

        return Redirect($"/Account/GetByUser/");
    }
    
    public async Task<IActionResult> Delete(Guid id)
    {
        await _accountService.Delete(id);
        return Redirect($"/Account/GetByUser/");
    }
}