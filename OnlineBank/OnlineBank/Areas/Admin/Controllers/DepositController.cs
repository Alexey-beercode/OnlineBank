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
        var depositViewModel = new UpdateDepositViewModel() { DepositId = depositId,DepositTypes = depositTypes,DepositTypeId = deposit.TypeId,Balance = deposit.Balance,InterestRate = deposit.InterestRate};
        return View(depositViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateDepositViewModel viewModel)
    {
        var deposit = await _depositService.GetById(viewModel.DepositId);
        deposit.InterestRate = viewModel.InterestRate;
        deposit.Balance = viewModel.Balance;
        if (viewModel.DepositTypeId != default)
        {
            deposit.TypeId = viewModel.DepositTypeId;
        }

        await _depositService.Update(deposit);
        return RedirectToAction("GetDeposits");
    }

    public async Task<IActionResult> Delete(Guid depositId)
    {
        await _depositService.Delete(depositId);
        return RedirectToAction("GetDeposits");
    }
}