using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBank.Data.ViewModels;
using OnlineBank.Service.Services;

namespace OnlineBank.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class DepositController:Controller
{
    private readonly DepositService _depositService;

    public DepositController(DepositService depositService)
    {
        _depositService = depositService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDeposits()
    {
        var deposits = await _depositService.GetDepositsAsync();
        return View(deposits);
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid depositId)
    {
        var deposit = await _depositService.GetById(depositId);
        var depositTypes = await _depositService.GetDepositTypes();
        var depositType = await _depositService.GetTypeByDepositId(depositId);
        var depositViewModel = new UpdateDepositViewModel() { DepositId = depositId,DepositTypes = depositTypes,DepositTypeId = deposit.TypeId,Balance = deposit.Balance,InterestRate = deposit.InterestRate};
        return View(depositViewModel);
    }
}