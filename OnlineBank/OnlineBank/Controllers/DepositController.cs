using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBank.Data.Entity;
using OnlineBank.Data.ViewModels;
using OnlineBank.Service.Service;
using OnlineBank.Service.Services;

namespace OnlineBank.Controllers;

[Authorize]
public class DepositController:Controller
{
    private readonly DepositService _depositService;
    private readonly UserService _userService;
    private readonly ClientService _clientService;

    public DepositController(DepositService depositService, UserService userService, ClientService clientService)
    {
        _depositService = depositService;
        _userService = userService;
        _clientService = clientService;
    }

    private async Task<Guid> GetClientId()
    {
        string userIdString = User.FindFirst("UserId")?.Value;
        var guidId = Guid.Parse(userIdString);
        var user = await _userService.GetByIdAsync(guidId);
        return user.ClientId;
    }
    public async Task<IActionResult> GetByUser()
    {
        try
        {
            string userIdString = User.FindFirst("UserId")?.Value;
            var guidId = Guid.Parse(userIdString);
            var user = await _userService.GetByIdAsync(guidId);
        
            if (user.ClientId.Equals(Guid.Empty))
            {
                return Redirect($"/User/CreateClient/");
            }
            
            var id = await GetClientId();
            
            var depositsByUser = await _depositService.GetDepositsByClientAsync(id);
            return View(depositsByUser);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        await _depositService.Delete(id);
        return Redirect($"/Deposit/GetByUser/");
    }

    [HttpGet]
    public async Task<IActionResult> UpToDeposit(Guid depositId)
    {
        var deposit = await _depositService.GetById(depositId);
        
        return View(new DepositOperationViewModel(){DepositId = depositId, Balance = deposit.Balance});
    }
    [HttpGet]
    public async Task<IActionResult> WithdrawFromDeposit(Guid depositId)
    {
        var deposit = await _depositService.GetById(depositId);
        
        return View(new DepositOperationViewModel(){DepositId = depositId, Balance = deposit.Balance});
    }

    [HttpPost]
    public async Task<IActionResult> WithdrawFromDeposit(DepositOperationViewModel depositOperationViewModel)
    {
        var deposits = await _depositService.WithdrawFromDeposit(depositOperationViewModel.DepositId,depositOperationViewModel.Amount,depositOperationViewModel.Note);
        var client = await _clientService.GetByIdAsync((await GetClientId()).ToString());
        var depositByClientViewModels = new DepositViewModel();

        depositByClientViewModels.Deposits = new List<DepositByClienViewModel>();
        foreach (var depositByClient in deposits)
        {
            var depositType = await _depositService.GetTypeByDepositId(depositByClient.Id);
            var totalAmountAtEnd = await _depositService.CalculateTotalAmount(depositByClient.Balance,
                (int)(depositByClient.Time.TotalDays / 30), depositType.Name);
            var depositByClientViewModel = new DepositByClienViewModel()
                { Deposit = depositByClient, DepositType = depositType ,TotalAmountAtEnd = totalAmountAtEnd};
            depositByClientViewModels.Deposits.Add(depositByClientViewModel);
        }

        depositByClientViewModels.ClientSurname = client.Surname;
        depositByClientViewModels.CLientName = client.Name;
        return View("GetByUser",depositByClientViewModels);

    }

    [HttpPost]
    public async Task<IActionResult> UpToDeposit(DepositOperationViewModel depositOperationViewModel)
    {
        var deposits = await _depositService.UpToDepositBalance(depositOperationViewModel.DepositId,depositOperationViewModel.Amount,depositOperationViewModel.Note);
        var client = await _clientService.GetByIdAsync((await GetClientId()).ToString());
        var depositByClientViewModels = new DepositViewModel();

        depositByClientViewModels.Deposits = new List<DepositByClienViewModel>();
        foreach (var depositByClient in deposits)
        {
            var depositType = await _depositService.GetTypeByDepositId(depositByClient.Id);
            var totalAmountAtEnd = await _depositService.CalculateTotalAmount(depositByClient.Balance,
                (int)(depositByClient.Time.TotalDays / 30), depositType.Name);
            var depositByClientViewModel = new DepositByClienViewModel()
                { Deposit = depositByClient, DepositType = depositType ,TotalAmountAtEnd = totalAmountAtEnd};
            depositByClientViewModels.Deposits.Add(depositByClientViewModel);
        }

        depositByClientViewModels.ClientSurname = client.Surname;
        depositByClientViewModels.CLientName = client.Name;
        return View("GetByUser",depositByClientViewModels);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        string userIdString = User.FindFirst("UserId")?.Value;
        var guidId = Guid.Parse(userIdString);
        var user = await _userService.GetByIdAsync(guidId);
        
        if (user.ClientId.Equals(Guid.Empty))
        {
            return Redirect($"/User/CreateClient/");
        }
        
        var types = await _depositService.GetDepositTypes();
        return View(new CreateDepositViewModel() { DepositTypes = types });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDepositViewModel viewModel)
    {
        var typeName = viewModel.SelectedDepositType;
        var time = DateTime.Now.AddMonths(viewModel.MounthCount) - DateTime.Now;
        var clientId =await GetClientId();
        var deposits = await _depositService.Create(typeName, clientId, time, viewModel.Balance);
        return RedirectToAction("GetByUser");
    }
}