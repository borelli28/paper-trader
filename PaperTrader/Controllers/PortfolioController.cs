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

        public PortfolioController(PaperTraderContext context)
        {
            _context = context;
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
                var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
                if (user == null)
                {
                    return NotFound();
                }
                else if (user.Id.ToString() != loggedInUserId)
                {
                    return Unauthorized();
                }

                return View(user);
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
        public async Task<IActionResult> Create([Bind("Id,Name")] Portfolio portfolio)
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
                return RedirectToAction("Home");
            }

            return View(portfolio);
        }

    }
}
