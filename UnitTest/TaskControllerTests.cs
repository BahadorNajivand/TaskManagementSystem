using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManagementSystem.Controllers;
using ModelsAndEnums.Models;
using TaskManagementSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using ModelsAndEnums.Enums;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TaskManagementSystem.Tests.Controllers
{
    [TestFixture]
    public class TasksControllerTests
    {
        private TasksController _controller;
        private WebApplicationDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WebApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TaskManagementSystemTest")
                .Options;

            _dbContext = new WebApplicationDbContext(options);
            _dbContext.Database.EnsureCreated();


            var httpContext = new DefaultHttpContext();
            httpContext.Session = new Mock<ISession>().Object;
            httpContext.Session.SetInt32("UserId", 1);

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Get the HttpContextAccessor instance
            var httpContextAccessor = httpContextAccessorMock.Object;

            _controller = new TasksController(_dbContext, httpContextAccessorMock.Object);
            _controller._userId = 1;
        }

        [TearDown]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async System.Threading.Tasks.Task Create_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var task = new TaskViewModel
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.Now.AddDays(7),
                Priority = TaskPriority.High,
                Status = ModelsAndEnums.Enums.TaskStatus.Open
            };

            // Act
            var result = await _controller.Create(task) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public async System.Threading.Tasks.Task Create_InvalidModel_ReturnsView()
        {
            // Arrange
            var task = new TaskViewModel
            {
                Title = "Test Task",
                Description = null, // Invalid model state
                DueDate = DateTime.Now.AddDays(7),
                Priority = TaskPriority.High,
                Status = ModelsAndEnums.Enums.TaskStatus.Open
            };
            _controller.ModelState.AddModelError("Description", "Description is required.");

            // Act
            var result = await _controller.Create(task) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(task, result.Model);
        }

        [Test]
        public async System.Threading.Tasks.Task Edit_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var existingTask = new ModelsAndEnums.Models.Task
            {
                TaskId = 1,
                Title = "Existing Task",
                Description = "Existing Description",
                DueDate = DateTime.Now.AddDays(7),
                Priority = TaskPriority.High,
                Status = ModelsAndEnums.Enums.TaskStatus.Open,
                AssigneeId = 1
            };
            await _dbContext.Tasks.AddAsync(existingTask);
            await _dbContext.SaveChangesAsync();

            var editedTask = new TaskViewModel
            {
                TaskId = existingTask.TaskId,
                Title = "Edited Task",
                Description = "Edited Description",
                DueDate = DateTime.Now.AddDays(14),
                Priority = TaskPriority.Medium,
                Status = ModelsAndEnums.Enums.TaskStatus.InProgress
            };

            // Act
            var result = await _controller.Edit(existingTask.TaskId, editedTask) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var updatedTask = await _dbContext.Tasks.FindAsync(existingTask.TaskId);
            Assert.AreEqual(editedTask.Title, updatedTask.Title);
            Assert.AreEqual(editedTask.Description, updatedTask.Description);
            Assert.AreEqual(editedTask.DueDate, updatedTask.DueDate);
            Assert.AreEqual(editedTask.Priority, updatedTask.Priority);
            Assert.AreEqual(editedTask.Status, updatedTask.Status);
        }

        [Test]
        public async System.Threading.Tasks.Task Edit_InvalidModel_ReturnsView()
        {
            // Arrange
            var existingTask = new ModelsAndEnums.Models.Task
            {
                TaskId = 1,
                Title = "Existing Task",
                Description = "Existing Description",
                DueDate = DateTime.Now.AddDays(7),
                Priority = TaskPriority.High,
                Status = ModelsAndEnums.Enums.TaskStatus.Open,
                AssigneeId = 1,
            };
            await _dbContext.Tasks.AddAsync(existingTask);
            await _dbContext.SaveChangesAsync();

            var editedTask = new TaskViewModel
            {
                TaskId = existingTask.TaskId,
                Title = "Edited Task",
                Description = null, // Invalid model state
                DueDate = DateTime.Now.AddDays(14),
                Priority = TaskPriority.Medium,
                Status = ModelsAndEnums.Enums.TaskStatus.InProgress
            };
            _controller.ModelState.AddModelError("Description", "Description is required.");

            // Act
            var result = await _controller.Edit(existingTask.TaskId, editedTask) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(editedTask, result.Model);

            var updatedTask = await _dbContext.Tasks.FindAsync(existingTask.TaskId);
            Assert.AreEqual(existingTask.Title, updatedTask.Title);
            Assert.AreEqual(existingTask.Description, updatedTask.Description);
            Assert.AreEqual(existingTask.DueDate, updatedTask.DueDate);
            Assert.AreEqual(existingTask.Priority, updatedTask.Priority);
            Assert.AreEqual(existingTask.Status, updatedTask.Status);
            Assert.AreEqual(existingTask.AssigneeId, updatedTask.AssigneeId);
        }
    }
}


public class SessionWrapper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionWrapper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetInt32(string key, int value)
    {
        _httpContextAccessor.HttpContext.Session.SetInt32(key, value);
    }

    // Add other methods for setting different types of session values if needed
}