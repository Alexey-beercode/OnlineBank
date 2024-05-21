using Microsoft.AspNetCore.Mvc;
using OnlineBank.Service.Service;

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
}