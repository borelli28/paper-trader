using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PaperTrader.Controllers;
using PaperTrader.Data;
using PaperTrader.Models;
using System.Threading.Tasks;

namespace PaperTrader.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        private UserController _controller;
        private PaperTraderContext _context;
        private IPasswordHasher<User> _passwordHasher;

        [SetUp]
        public void Setup()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<PaperTraderContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new PaperTraderContext(options);

            // Add test data
            _context.User.Add(new User { Id = 1, Username = "existinguser", Password = "hashedpassword" });
            _context.SaveChanges();

            // Use real password hasher
            _passwordHasher = new PasswordHasher<User>();

            _controller = new UserController(_context, _passwordHasher);
        }

        [Test]
        public async Task Register_ValidUser_RedirectsToLogin()
        {
            // Arrange
            var user = new User { Username = "newuser", Password = "password" };

            // Act
            var result = await _controller.Register(user) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Login"));
            Assert.That(result.ControllerName, Is.EqualTo("User"));
        }

        [Test]
        public async Task Register_ExistingUsername_ReturnsViewWithModelError()
        {
            // Arrange
            var user = new User { Username = "existinguser", Password = "password" };

            // Act
            var result = await _controller.Register(user) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewData.ModelState.ContainsKey("Username"), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _controller.Dispose();
        }
    }
}
