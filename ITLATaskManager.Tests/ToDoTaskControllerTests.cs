using ITLATaskManager.DataAccess.Data;
using ITLATaskManager.Models;
using ITLATaskManagerAPI.Controllers;
using ITLATaskManagerAPI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITLATaskManager.Tests
{
    public class ToDoTaskControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ToDoTaskController _controller;
        private readonly Mock<IHubContext<NotificationHub>> _hubContextMock;

        public ToDoTaskControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _controller = new ToDoTaskController(_context, _hubContextMock.Object);
        }

        [Fact]
        public async Task GetTasks_ReturnsListOfTasks()
        {
            var tasks = new List<ToDoTask<string>> { CreateValidTask(), CreateValidTask() };
            _context.ToDoTasks.AddRange(tasks);
            await _context.SaveChangesAsync();

            var result = await _controller.GetTasks();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetTaskById_WithExistingId_ReturnsOkWithTask()
        {
            var task = CreateValidTask();
            _context.ToDoTasks.Add(task);
            await _context.SaveChangesAsync();

            var result = await _controller.GetTaskById(task.Id);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetTaskById_WithNonExistingId_ReturnsNotFound()
        {
            var result = await _controller.GetTaskById(999);

            Assert.IsType<NotFoundResult>(result);
        }

        private static ToDoTask<string> CreateValidTask()
        {
            return new ToDoTask<string>
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.Now.AddDays(1),
                Status = "Pending",
                AdditionalData = "Test Data",
            };
        }

        private void Dispose() => _context.Dispose();
    }
}
