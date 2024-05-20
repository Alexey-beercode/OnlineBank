using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBank.Data.ViewModel;
using OnlineBank.Service.Service;
using OnlineBank.Service.Services;

namespace OnlineBank.Controllers;

public class UserController : Controller
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Authorization()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Authorization");
        }
        
        try
        {
            var response = await _userService.LoginAsync(model);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response));
    
            return RedirectToAction("Index","Home");
        }
        catch(Exception ex)
        {
            ModelState.AddModelError("AuthorizationError", ex.Message);

            return View("Authorization");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Authorization");
        }
        
        try
        {
            var response = await _userService.RegisterAsync(model,"Resident");
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response));
            
            return Redirect("/");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("AuthorizationError", ex.Message);

            return View("Authorization");
        }
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}