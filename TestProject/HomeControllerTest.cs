
using Lab8.Areas.Identity.Data;
using Lab8.Controllers;
using Lab8.Data;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Web.Mvc;

namespace TestProject
{
    public class HomeControllerTest
    {
        private readonly Mock<CommunityStoreContext> _context;
        private readonly Mock<AuthenticationContext> _userManager;
        private readonly HomeController _controller;

        public HomeControllerTest()
        {
            _context = new Mock<CommunityStoreContext>();
            _userManager = new Mock<AuthenticationContext>();
            _controller = new HomeController(_context);
        }

        [Fact]
        public void IndexReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController(_context);
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public void PrivacyReturnsViewPrivacy()
        {
            // Arrange
            var controller = new HomeController(_context);
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public void ErrorReturnsViewError()
        {
            // Arrange
            var controller = new HomeController(_context);
            // Act
            var result = _controller.Error();

            // Assert
            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public void Claim_UnclaimWorks()
        {

            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);

        }
    }
}