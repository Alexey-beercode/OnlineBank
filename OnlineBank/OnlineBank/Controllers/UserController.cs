using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBank.Data.ViewModel;
using OnlineBank.Models;
using OnlineBank.Service.Service;
using OnlineBank.Service.Services;

namespace OnlineBank.Controllers;

public class UserController : Controller
{
    private readonly UserService _userService;
    private readonly ClientService _clientService;
    public UserController(UserService userService, ClientService clientService)
    {
        _userService = userService;
        _clientService = clientService;
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
    
            return Redirect("/");
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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        try
        {
            string login = User.Identity.Name;
            if (string.IsNullOrEmpty(login))
            {
                throw new Exception("User not found");
            }
            
            var currentUser = await _userService.GetByLogin(login);
            if (currentUser is null)
            {
                throw new Exception("User not found");
            }

            if (currentUser.ClientId.Equals(Guid.Empty))
            {
                return Redirect($"/User/CreateClient/");
            }

            return View();
        }
        catch (Exception ex)
        {
            return View("Error", new ErrorViewModel() { RequestId = ex.Message });
        }
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> CreateClient()
    {
        return View();
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateClient(ClientViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            
            string userIdString = User.FindFirst("UserId")?.Value;
            await _clientService.CreateClientAsync(viewModel, userIdString);
            
            return Redirect($"/User/Profile/"); 
        }
        catch (Exception ex)
        {
            return View("Error", new ErrorViewModel() { RequestId = ex.Message });
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
