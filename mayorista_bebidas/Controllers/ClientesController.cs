using mayorista_bebidas.Services;
using Microsoft.AspNetCore.Mvc;

namespace mayorista_bebidas.Controllers;

public class ClientesController : Controller
{
    private readonly ClienteService _clienteService;

    public ClientesController(ClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(bool soloActivos = false)
    {
        ViewBag.SoloActivos = soloActivos;
        var clientes = await _clienteService.ObtenerTodosAsync(soloActivos);
        return View(clientes);
    }
}
