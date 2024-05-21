using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBank.Data.ViewModel;
using OnlineBank.Data.ViewModels;
using OnlineBank.Models;
using OnlineBank.Service.Service;
using OnlineBank.Service.Services;

namespace OnlineBank.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class UserController : Controller
{
    private readonly UserService _userService;
    private readonly RoleService _roleService;

    public UserController(UserService userService, RoleService roleService)
    {
        _userService = userService;
        _roleService = roleService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _userService.GetAll();
            var usersViewModel = new UsersViewModel();
            foreach (var user in users)
            {
                var roles = await _userService.GetRolesByUser(user.Id);
                usersViewModel.UserViewModels.Add(new UserViewModel(){Roles = roles,User = user});
            }

            usersViewModel.Roles = await _roleService.GetAll();
            return View(usersViewModel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid userId)
    {
        var user = await _userService.GetByIdAsync(userId);
        var rolesByUser = await _userService.GetRolesByUser(userId);
        var roles = await _roleService.GetAll();
        var userModel = new UserViewModel() { Roles = roles, User = user };
        var listUserModel = new List<UserViewModel>();
        listUserModel.Add(new UserViewModel(){Roles = rolesByUser,User = user});
        return View(new UsersViewModel(){Roles = roles, UserViewModels = listUserModel});
    }
}
