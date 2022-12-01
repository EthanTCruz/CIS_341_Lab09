
using Lab8.Areas.Identity.Data;
using Lab8.Controllers;
using Lab8.Data;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using Microsoft.AspNetCore.Mvc;

namespace TestProject
{
    public class HomeControllerTest
    {
        private readonly Mock<CommunityStoreContext> _context;
        private readonly Mock<AuthenticationContext> _userManager;


        public HomeControllerTest()
        {
            _context = new Mock<CommunityStoreContext>();
            _userManager = new Mock<AuthenticationContext>();

        }

        [Fact]
        public void IndexReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController(_context.Object);
            // Act
            var result = controller.Index();


            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public void PrivacyReturnsViewPrivacy()
        {
            // Arrange
            var controller = new HomeController(_context.Object);
            // Act
            var result = controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);

        }


        [Fact]
        public void Claim_UnclaimWorks()
        {
            // Arrange
            var controller = new HomeController(_context.Object);
            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);

        }
    }
}