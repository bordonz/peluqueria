using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using peluqueria.Models;

namespace peluqueria.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Restringido()
    {
        ViewData["Title"] = "Acceso denegado";
        return View();
    }
}
