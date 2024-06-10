using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Diagnostics;
using PaperTrader.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PaperTrader.Controllers;

public class AppController : Controller
{
    private readonly ILogger<AppController> _logger;

    public AppController(ILogger<AppController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Home()
    {
        if (User.Identity.IsAuthenticated)
        {
            return View();
        }
        else
        {
            return RedirectToAction("Login", "User");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}