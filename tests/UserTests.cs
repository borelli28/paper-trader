using NUnit.Framework;
using PaperTrader.Controllers;
using PaperTrader.Data;
using PaperTrader.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace PaperTrader.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        private UserController _controller;
        private Mock<PaperTraderContext> _mockContext;
        private Mock<IPasswordHasher<User>> _mockPasswordHasher;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<PaperTraderContext>();
            _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            _controller = new UserController(_mockContext.Object, _mockPasswordHasher.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task Register_ValidUser_RedirectsToLogin()
        {
            // Arrange
            var user = new User { Username = "testuser", Password = "password" };
            _mockContext.Setup(c => c.User.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

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
            _mockContext.Setup(c => c.User.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User());

            // Act
            var result = await _controller.Register(user) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewData.ModelState.ContainsKey("Username"), Is.True);
        }

        // Add more tests here...
    }
}
