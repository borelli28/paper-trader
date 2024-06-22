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
    public class PortfolioController : Controller
    {
        private readonly PaperTraderContext _context;
        private readonly ILogger<PortfolioController> _logger;

        public PortfolioController(PaperTraderContext context, ILogger<PortfolioController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.User
                    .Include(u => u.Portfolios) // Include Portfolios navigation property
                    .FirstOrDefaultAsync(u => u.Id.ToString() == loggedInUserId);

                if (user == null)
                {
                    return NotFound();
                }

                return View(user.Portfolios);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.User.FirstOrDefaultAsync(m => m.Id.ToString() == loggedInUserId);
                var portfolio = await _context.Portfolio.FirstOrDefaultAsync(m => m.Id == id);
                if (portfolio == null)
                {
                    return NotFound();
                }
                else if (portfolio.UserId.ToString() != loggedInUserId)
                {
                    return Unauthorized();
                }

                return View(portfolio);
            } 
            else 
            {
                return RedirectToAction("Login", "User");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Portfolio portfolio)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Modelstate is valid");
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.User.FirstOrDefaultAsync(u => u.Id.ToString() == loggedInUserId);

                if (user == null)
                {
                    return NotFound();
                }

                var existingPortfolio = await _context.Portfolio
                    .FirstOrDefaultAsync(p => p.Name == portfolio.Name && p.UserId == user.Id);

                if (existingPortfolio != null)
                {
                    ModelState.AddModelError("Name", "Please choose a different Name.");
                    return View(portfolio);
                }

                portfolio.UserId = user.Id;
                _context.Portfolio.Add(portfolio);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Portfolio created successfully!";
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

            return View(portfolio);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (User.Identity.IsAuthenticated) 
            {
                if (id == null)
                {
                    return NotFound();
                }

                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.User.FirstOrDefaultAsync(m => m.Id.ToString() == loggedInUserId);
                var portfolio = await _context.Portfolio.FirstOrDefaultAsync(m => m.Id == id);
                if (portfolio == null)
                {
                    return NotFound();
                }
                else if (portfolio.UserId.ToString() != loggedInUserId)
                {
                    return Unauthorized();
                }

                return View(portfolio);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Cash")] Portfolio portfolio)
        {
            if (User.Identity.IsAuthenticated)
            {
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (portfolio.UserId.ToString() != loggedInUserId)
                {   
                    return Unauthorized();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(portfolio);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Portfolio updated successfully!";
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PortfolioExists(portfolio.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction("Details", new { id = portfolio.Id });
                }
                return View(portfolio);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        
        private bool PortfolioExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

    }
}
