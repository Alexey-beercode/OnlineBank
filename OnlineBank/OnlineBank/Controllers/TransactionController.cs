using Microsoft.AspNetCore.Mvc;
using OnlineBank.Data.ViewModels;
using OnlineBank.Service.Services;

namespace OnlineBank.Controllers;

public class TransactionController:Controller
{
    private readonly TransactionService _transactionService;
    private readonly UserService _userService;

    public TransactionController(TransactionService transactionService, UserService userService)
    {
        _transactionService = transactionService;
        _userService = userService;
    }

    private async Task<Guid> GetClientId()
    {
        string userIdString = User.FindFirst("UserId")?.Value;
        var guidId = Guid.Parse(userIdString);
        var user = await _userService.GetByIdAsync(guidId);
        return user.ClientId;
    }
    [HttpGet]
    public async Task<IActionResult> GetByUser()
    {
        var clientId =await GetClientId();
        var transactions = await _transactionService.GetByClienIdAsync(clientId);
        return View(transactions);
    }

    [HttpGet]
    public async Task<IActionResult> GetByAccount(Guid accountId)
    {
        var transactions = await _transactionService.GetByDepositOrAccount(await GetClientId(), accountId: accountId);
        return View("GetByUser", transactions);
    }

    [HttpGet]
    public async Task<IActionResult> GetByDeposit(Guid depositId)
    {
        var transactions = await _transactionService.GetByDepositOrAccount(await GetClientId(), depositId:depositId);
        return View("GetByUser", transactions);
    }
}