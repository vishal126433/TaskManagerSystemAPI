using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Interfaces;
using AuthService.Controllers;
using AuthService.Models;
using TaskManager.DTOs;
using TaskManager.Helpers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TaskManagerTest.Controllers
{
    public class UsersControllerTest
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UsersController _controller;

        public UsersControllerTest()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UsersController(_userServiceMock.Object);
        }


        [Fact]
        public async Task RefreshToken_ReturnsUnauthorized_WhenTokenIsMissing()
        {
            var context = new DefaultHttpContext(); // No cookie at all

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            var result = await _controller.RefreshToken();

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorized.StatusCode);
        }



        [Fact]
        public async Task RefreshToken_ReturnsNewToken_WhenTokenIsValid()
        {
            var expectedToken = new TokenResponse { AccessToken = "newtoken", RefreshToken = "refresh" };

            var context = new DefaultHttpContext();
            context.Request.Headers["Cookie"] = "refreshToken=validtoken";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            _userServiceMock.Setup(s => s.RefreshTokenAsync("validtoken")).ReturnsAsync(expectedToken);

            var result = await _controller.RefreshToken();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }


        [Fact]
        public async Task GetAllUsers_ReturnsUsersList()
        {
            _userServiceMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(new List<User> { new User() });
            var result = await _controller.GetAllUsers();
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task ToggleActiveStatus_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _userServiceMock.Setup(x => x.ToggleUserActiveStatusAsync(It.IsAny<int>())).ReturnsAsync((bool?)null);
            var result = await _controller.ToggleActiveStatus(1);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }

        [Fact]
        public async Task ToggleActiveStatus_ReturnsStatus_WhenUserExists()
        {
            _userServiceMock.Setup(x => x.ToggleUserActiveStatusAsync(1)).ReturnsAsync(true);
            var result = await _controller.ToggleActiveStatus(1);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task CreateUser_ReturnsBadRequest_WhenRequiredFieldsMissing()
        {
            var request = new RegisterRequest { Username = "", Email = "", Password = "" };
            var result = await _controller.CreateUser(request);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task CreateUser_ReturnsOk_WhenUserCreated()
        {
            var request = new RegisterRequest { Username = "user", Email = "email@test.com", Password = "pass" };
            var user = new User { Username = "user", Email = "email@test.com", Role = "User" };
            _userServiceMock.Setup(x => x.CreateUserAsync(request)).ReturnsAsync(user);
            var result = await _controller.CreateUser(request);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFound_WhenUserNotExists()
        {
            _userServiceMock.Setup(x => x.UpdateUserAsync(1, It.IsAny<User>())).ReturnsAsync((User)null);
            var result = await _controller.UpdateUser(1, new User());
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOk_WhenUserUpdated()
        {
            var updatedUser = new User { Id = 1, Username = "updated" };
            _userServiceMock.Setup(x => x.UpdateUserAsync(1, It.IsAny<User>())).ReturnsAsync(updatedUser);
            var result = await _controller.UpdateUser(1, updatedUser);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserNotExists()
        {
            _userServiceMock.Setup(x => x.DeleteUserAsync(1)).ReturnsAsync(false);
            var result = await _controller.DeleteUser(1);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_ReturnsOk_WhenDeleted()
        {
            _userServiceMock.Setup(x => x.DeleteUserAsync(1)).ReturnsAsync(true);
            var result = await _controller.DeleteUser(1);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }
    }
}
