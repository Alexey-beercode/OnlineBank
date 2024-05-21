using Microsoft.AspNetCore.Mvc;
using OnlineBank.Data.ViewModel;
using OnlineBank.Data.ViewModels;
using OnlineBank.Service.Service;
using OnlineBank.Service.Services;

namespace OnlineBank.Areas.Admin.Controllers;

[Area("Admin")]
public class ClientController:Controller
{
    private readonly ClientService _clientService;

    public ClientController(ClientService clientService)
    {
        _clientService = clientService;
    }

    public async Task<IActionResult> GetAll()
    {
        try
        {
            var clients = await _clientService.GetAll();
            return View(clients);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid clientId)
    {
        var client = await _clientService.GetByIdAsync(clientId.ToString());
        return View(new UpdateClientViewModel()
            { Address = client.Address, BirthDay = client.BirthDay, Name = client.Name, Surname = client.Surname,Id = clientId});
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateClientViewModel viewModel)
    {
        var client = await _clientService.GetByIdAsync(viewModel.Id.ToString());
        client.Address = viewModel.Address;
        client.Name = viewModel.Name;
        client.Surname = viewModel.Surname;
        client.BirthDay = viewModel.BirthDay;
        await _clientService.Update(client);
        return RedirectToAction("GetAll");
    }

    public async Task<IActionResult> Delete(Guid clientId)
    {
        await _clientService.DeleteClientAsync(clientId.ToString());
        return RedirectToAction("GetAll");
    }

    [HttpGet]
    public async Task<IActionResult> SearchClients(string name)
    {
        var clients = await _clientService.SearchByName(name);
        return View("GetAll", clients);
    }
}