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
                var userViewModel = new UserViewModel() { Roles = roles, User = user };
                usersViewModel.UserViewModels.Add(userViewModel);
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
        var roles = await _roleService.GetAll();
        
        return View(new UpdateUserViewModel(){Roles = roles,User = user});
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateUserViewModel viewModel)
    {
        var roleid = viewModel.NewRoleId;
        var userId = viewModel.User.Id;
        var role = (await _roleService.GetAll()).FirstOrDefault(a => a.Id == roleid);
        var isUserHasRole = await _userService.CheckUserHasRole(userId, role.Name);
        if (!isUserHasRole)
        {
            await _roleService.SetRoleToUser(role.Name,userId);
           
        }
        var login = viewModel.User.Login;
        var user = await _userService.GetByIdAsync(userId);
        user.Login = login;
        await _userService.Update(user);
        return RedirectToAction("GetUsers");
        
    }

    public async Task<IActionResult> Delete(Guid userId)
    {
        await _userService.Delete(userId);
        return RedirectToAction("GetUsers");
    }
}
