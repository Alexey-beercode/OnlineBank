using Microsoft.AspNetCore.Mvc;
using OnlineBank.Service.Services;

namespace OnlineBank.Areas.Admin.Controllers;

[Area("Admin")]
public class AccountController : Controller
{
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var accounts = await _accountService.GetAllWithClient(); 
        
        return View(accounts);
    }
}