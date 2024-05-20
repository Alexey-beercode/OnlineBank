using Microsoft.AspNetCore.Mvc;

namespace OnlineBank.Controllers;

public class AccountController : Controller
{
    public IActionResult GetByUser()
    {
        return View();
    }
}