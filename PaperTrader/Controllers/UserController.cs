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
    public class UserController : Controller
    {
        private readonly PaperTraderContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserController(PaperTraderContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.User.FirstOrDefaultAsync(u => u.Id.ToString() == loggedInUserId);
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

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,Username,Password,CreatedAt")] User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Username == user.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Please choose a different username.");
                    return View(user);
                }

                user.Password = _passwordHasher.HashPassword(user, user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToAction("Login", "User");
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                var userFromDb = await _context.User.FirstOrDefaultAsync(u => u.Username == user.Username);
                if (userFromDb != null)
                {
                    var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(userFromDb, userFromDb.Password, user.Password);
                    if (passwordVerificationResult == PasswordVerificationResult.Success)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userFromDb.Username),
                            new Claim(ClaimTypes.NameIdentifier, userFromDb.Id.ToString())
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                        return RedirectToAction("Home", "App");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }
            return View(user);
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
                var user = await _context.User.FindAsync(id);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,CreatedAt")] User user)
        {
            if (User.Identity.IsAuthenticated)
            {
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (id != user.Id)
                {
                    return NotFound();
                }

                if (user.Id.ToString() != loggedInUserId)
                {   
                    return Unauthorized();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        user.Password = _passwordHasher.HashPassword(user, user.Password);
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "User updated successfully!";
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserExists(user.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(user);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public async Task<IActionResult> Delete(int? id)
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.User.FindAsync(id);

                if (user == null)
                {
                    return NotFound();
                }
                else if (user.Id.ToString() != loggedInUserId)
                {
                    return Unauthorized();
                }
                else 
                {
                    _context.User.Remove(user);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "User deleted successfully!";
                }
                return RedirectToAction("Index", "App");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
