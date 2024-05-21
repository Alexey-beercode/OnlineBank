using Microsoft.AspNetCore.Mvc;
using OnlineBank.Service.Services;

namespace OnlineBank.Areas.Admin.Controllers;

[Area("Admin")]
public class TransactionController : Controller
{
    private readonly TransactionService _transactionService;

    public TransactionController(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var transactions = await _transactionService.GetAll(); 
        
        return View(transactions);
    }
}