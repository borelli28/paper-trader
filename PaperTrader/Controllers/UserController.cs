using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaperTrader.Data;
using PaperTrader.Models;
using NuGet.Packaging.Signing;
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
                var loggedUsername = User.Identity.Name;
                var user = await _context.User.FirstOrDefaultAsync(u => u.Username == loggedUsername);
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

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var loggedUsername = User.Identity.Name;
                var user = await _context.User
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (user == null)
                {
                    return NotFound();
                }
                else if (user.Username != loggedUsername)
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

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (User.Identity.IsAuthenticated) 
            {
                if (id == null)
                {
                    return NotFound();
                }
                var loggedUsername = User.Identity.Name;
                var user = await _context.User.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                else if (user.Username != loggedUsername)
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

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,CreatedAt")] User user)
        {
            if (User.Identity.IsAuthenticated)
            {
                string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (id != user.Id)
                {
                    return NotFound();
                }

                if (user.Id.ToString() != loggedInUserId)
                {   
                    return Unauthorized();
                }
                /*
                    {"message":"User ID: 18 and logged in user ID: "}
                    loggedInUserId is returning null because the user id is not being set on login on the claims thingy
                    Need to find another way to get the logged user id or maybe implement the code chatgpt gave me
                */

                if (ModelState.IsValid)
                {
                    try
                    {
                        user.Password = _passwordHasher.HashPassword(user, user.Password);
                        _context.Update(user);
                        await _context.SaveChangesAsync();
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

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {   if (User.Identity.IsAuthenticated)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var loggedUsername = User.Identity.Name;
                var user = await _context.User
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (user == null)
                {
                    return NotFound();
                }
                else if (user.Username != loggedUsername)
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

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var loggedUsername = User.Identity.Name;
                var user = await _context.User.FindAsync(id);

                if (user == null)
                {
                    return NotFound();
                }
                else if (user.Username != loggedUsername)
                {
                    return Unauthorized();
                }
                else {
                    _context.User.Remove(user);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
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
