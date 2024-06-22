using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Diagnostics;
using PaperTrader.Data;
using PaperTrader.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace PaperTrader.Controllers;

public class AppController : Controller
{
    private readonly ILogger<AppController> _logger;
    private readonly PaperTraderContext _context;

    public AppController(ILogger<AppController> logger, PaperTraderContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Home()
    {
        if (User.Identity.IsAuthenticated)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.User
                                    .Include(u => u.Portfolios)
                                    .FirstOrDefaultAsync(u => u.Id.ToString() == loggedInUserId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
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