using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaperTrader.Data;
using PaperTrader.Models;
using Microsoft.AspNetCore.Identity;

namespace PaperTrader.Controllers
{
    public class StockController : Controller
    {
        private readonly PaperTraderContext _context;
        private readonly ILogger<StockController> _logger;

        public StockController(PaperTraderContext context, ILogger<StockController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public IActionResult Create(int id)
        {
            ViewBag.portfolioId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StockTicker,Name,SharesTotal,ShareAvgPurchasePrice")] Stock stock, int portfolioId)
        {
            if (ModelState.IsValid)
            {
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.User.FirstOrDefaultAsync(u => u.Id.ToString() == loggedInUserId);

                if (user == null)
                {
                    return NotFound();
                }

                var existingPortfolio = await _context.Portfolio
                    .FirstOrDefaultAsync(p => p.Id == portfolioId && p.UserId == user.Id);

                if (existingPortfolio == null)
                {
                    _logger.LogWarning($"ModelState Error: Could not find portfolio");
                    TempData["ErrorMessage"] = "Could not find portfolio";
                    return View(stock);
                }

                stock.PortfolioId = existingPortfolio.Id;
                _context.Stock.Add(stock);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Stock added to portfolio!";
                return RedirectToAction("Home", "App");
            }
            else
            {
                _logger.LogWarning("ModelState is invalid");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogWarning($"ModelState Error: {error.ErrorMessage}");
                    }
                }
            }
            return View(stock);
        }
        
        private bool StockExists(int id)
        {
            return _context.Stock.Any(e => e.Id == id);
        }
    }
}
