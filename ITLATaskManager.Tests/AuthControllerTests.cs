using ITLATaskManager.DataAccess.Data;
using ITLATaskManager.Models;
using ITLATaskManagerAPI.Controllers;
using ITLATaskManagerAPI.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITLATaskManager.Tests
{
    public class AuthControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthController _controller;
        private readonly Mock<IAuthService> _authServiceMock;

        public AuthControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object, _context);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "password123",
            };
            _authServiceMock
                .Setup(x => x.GetToken(loginRequest.Email, loginRequest.Password))
                .ReturnsAsync("valid-jwt-token");

            var result = await _controller.Login(loginRequest);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
        {
            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "wrongpassword",
            };
            _authServiceMock
                .Setup(x => x.GetToken(loginRequest.Email, loginRequest.Password))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            var result = await _controller.Login(loginRequest);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateUser_WithValidData_ReturnsOkWithUser()
        {
            var user = new User
            {
                Email = "newuser@example.com",
                Password = "password123",
                Role = "User",
            };

            var result = await _controller.CreateUser(user);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var createdUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal("newuser@example.com", createdUser.Email);
        }

        [Fact]
        public async Task CreateUser_WithInvalidRole_ReturnsBadRequest()
        {
            var user = new User
            {
                Email = "test@example.com",
                Password = "password123",
                Role = "InvalidRole",
            };

            var result = await _controller.CreateUser(user);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Role must be either 'Admin' or 'User'", badRequestResult.Value);
        }

        private void Dispose() => _context.Dispose();
    }
}
