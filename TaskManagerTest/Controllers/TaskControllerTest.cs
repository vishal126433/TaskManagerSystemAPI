using Xunit;
using Moq;
using TaskManager.Interfaces;
using AuthService.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskManager.Services.FileUpload.Interfaces;
using TaskManager.Models;
using TaskManager.Helpers;
using TaskManager.DTOs;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Text.Json;
using System.Linq;
using AuthService.Models;
using System.Text;
using TaskManager.Services.FileUpload.Models;

public class TasksControllerTest
{
    private readonly TasksController _controller;
    private readonly Mock<ITaskUploadService> _taskUploadServiceMock;
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly Mock<ITaskStateService> _taskStateServiceMock;
    private readonly Mock<IUserService> _userServiceMock;

    public TasksControllerTest()
    {
        _taskUploadServiceMock = new Mock<ITaskUploadService>();
        _taskServiceMock = new Mock<ITaskService>();
        _taskStateServiceMock = new Mock<ITaskStateService>();
        _userServiceMock = new Mock<IUserService>();

        _controller = new TasksController(
            _taskUploadServiceMock.Object,
            _taskServiceMock.Object,
            _taskStateServiceMock.Object,
            _userServiceMock.Object
        );
    }


    [Fact]
    public async Task CreateTask_ReturnsOk_WithCreatedTask()
    {
        var taskToCreate = new TaskItem { Id = 1, Name = "Sample Task" };
        _taskServiceMock.Setup(x => x.CreateTaskAsync(It.IsAny<int>(), It.IsAny<TaskItem>()))
            .ReturnsAsync(taskToCreate);

        var result = await _controller.CreateTask(1, taskToCreate);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
        Assert.Equal(200, apiResponse.StatusCode);
    }
    [Fact]
    public async Task CreateTask_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        var task = new TaskItem { Id = 1, Name = "Invalid Task" };
        _taskServiceMock.Setup(x => x.CreateTaskAsync(It.IsAny<int>(), It.IsAny<TaskItem>()))
            .ThrowsAsync(new ArgumentException("Invalid task data"));

        var result = await _controller.CreateTask(1, task);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
        Assert.False(apiResponse.Success);
        Assert.Equal(400, apiResponse.StatusCode);
        Assert.Equal("Invalid task data", apiResponse.Message); // This will now pass
        Assert.Contains("Invalid task data", apiResponse.Errors);
    }


    [Fact]
    public async Task GetStatusList_ReturnsOk()
    {
        _taskServiceMock.Setup(x => x.GetStatusListAsync()).ReturnsAsync(new List<string> { "new", "completed", "in progress" });

        var result = await _controller.GetStatusList();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
        Assert.NotNull(response.Data);
    }

    [Fact]
    public async Task GetPriorityList_ReturnsOk()
    {
        _taskServiceMock.Setup(x => x.GetPriorityListAsync()).ReturnsAsync(new List<string> { "high", "low", "medium" });

        var result = await _controller.GetPriorityList();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetTypeList_ReturnsOk()
    {
        _taskServiceMock.Setup(x => x.GetTypeListAsync()).ReturnsAsync(new List<string> { "bug", "feature" });

        var result = await _controller.GetTypeList();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task SearchTasks_ByUser_ReturnsOk()
    {
        _taskServiceMock.Setup(x => x.SearchTasksByUserAsync(1, "test"))
            .ReturnsAsync(new List<TaskItem>());

        var result = await _controller.SearchTasks(1, "test");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task SearchTasks_ByUser_ReturnsBadRequest_OnEmptyQuery()
    {
        var result = await _controller.SearchTasks(1, "");

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(badResult.Value);
        Assert.Contains("Query cannot be empty.", response.Errors); // Check the error in the list
    }

    [Fact]
    public async Task SearchTasks_Global_ReturnsOk()
    {
        _taskServiceMock.Setup(x => x.SearchTasksAsync("test"))
            .ReturnsAsync(new List<TaskItem>());

        var result = await _controller.SearchTasks("test");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task SearchTasks_Global_ReturnsBadRequest_OnEmptyQuery()
    {
        var result = await _controller.SearchTasks("");

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(badResult.Value);

        Assert.Contains("Query cannot be empty.", response.Errors); // Check the error in the list
    }



    [Fact]
    public async Task UploadJson_ReturnsOk()
    {
        var taskDtoList = new List<TaskImport>
        {
            new TaskImport { Name = "Test", DueDate = "2025-01-01", Status = "new", Type = "bug", Priority = "high" }
        };

        _taskServiceMock.Setup(x => x.CreateTaskAsync(It.IsAny<int>(), It.IsAny<TaskItem>()))
            .ReturnsAsync(new TaskItem());

        _userServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new User { Id = 1, Username = "testuser" });

        var result = await _controller.UploadJson(taskDtoList);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task UploadJson_ReturnsBadRequest_WhenListIsNull()
    {
        var result = await _controller.UploadJson(null);

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(badResult.Value);
        Assert.Contains("No Task Provided.", response.Errors[0]);
    }

    [Fact]
    public async Task UploadJson_AddsError_WhenDueDateInvalid()
    {
        // Arrange
        var tasks = new List<TaskImport>
    {
        new TaskImport { Name = "Invalid Date", DueDate = "invalid-date" }
    };

        // Act
        var result = await _controller.UploadJson(tasks);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

        Assert.True(response.Success);
        Assert.Equal(200, response.StatusCode);

        // Deserialize Data to JsonDocument to access fields
        var json = JsonSerializer.Serialize(response.Data);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(0, root.GetProperty("successCount").GetInt32());
        Assert.Equal(1, root.GetProperty("errorCount").GetInt32());
    }


    [Fact]
    public async Task UploadJson_AddsError_WhenAssignToNotNumeric()
    {
        // Arrange
        var tasks = new List<TaskImport>
    {
        new TaskImport { Name = "Non-numeric", DueDate = "2025-01-01", AssignTo = "abc" }
    };

        // Act
        var result = await _controller.UploadJson(tasks);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

        Assert.True(response.Success);
        Assert.Equal(200, response.StatusCode);

        // Deserialize Data to JsonDocument to access dynamic fields
        var json = JsonSerializer.Serialize(response.Data);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(0, root.GetProperty("successCount").GetInt32());
        Assert.Equal(1, root.GetProperty("errorCount").GetInt32());
    }


    [Fact]
    public async Task UploadJson_AddsError_WhenUserNotFound()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);

        var tasks = new List<TaskImport>
    {
        new TaskImport { Name = "User Not Found", DueDate = "2025-01-01", AssignTo = "999" }
    };

        // Act
        var result = await _controller.UploadJson(tasks);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(200, response.StatusCode);

        // Serialize response.Data and re-parse to access properties
        var json = JsonSerializer.Serialize(response.Data);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(0, root.GetProperty("successCount").GetInt32());
        Assert.Equal(1, root.GetProperty("errorCount").GetInt32());
    }

    [Fact]
    public async Task UploadJson_AddsError_WhenCreateTaskThrows()
    {
        var tasks = new List<TaskImport>
    {
        new TaskImport { Name = "TaskWithException", DueDate = "2025-01-01", Status = "new", Type = "bug", Priority = "high" }
    };

        _userServiceMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new User { Id = 1, Username = "user" });

        _taskServiceMock.Setup(x => x.CreateTaskAsync(It.IsAny<int>(), It.IsAny<TaskItem>()))
            .ThrowsAsync(new Exception("Simulated failure"));

        var result = await _controller.UploadJson(tasks);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

        var json = JsonSerializer.Serialize(response.Data);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(0, root.GetProperty("successCount").GetInt32());
        Assert.Equal(1, root.GetProperty("errorCount").GetInt32());
        var errorArray = root.GetProperty("errors");
        Assert.Contains("Simulated failure", errorArray[0].GetString());
    }
    [Fact]
    public async Task UploadJson_ReturnsBadRequest_WhenListIsEmpty()
    {
        var result = await _controller.UploadJson(new List<TaskImport>());

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(badResult.Value);
        Assert.Contains("No Task Provided.", response.Errors[0]);
    }

    [Fact]
    public async Task GetTasksByUserId_ReturnsOk()
    {
        // Arrange
        var mockTasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Name = "Test Task 1" },
            new TaskItem { Id = 2, Name = "Test Task 2" }
        };

        _taskServiceMock
            .Setup(x => x.GetTasksByUserIdAsync(1))
            .ReturnsAsync(mockTasks);

        // Act
        var result = await _controller.GetTasksByUserId(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<List<TaskItem>>>(okResult.Value);

        Assert.True(response.Success);
        Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data);

        var tasks = Assert.IsAssignableFrom<List<TaskItem>>(response.Data);
        Assert.Equal(2, tasks.Count);
    }

    [Fact]
    public async Task GetTasksByUserId_ReturnsOk_WithEmptyList()
    {
        // Arrange
        _taskServiceMock
            .Setup(x => x.GetTasksByUserIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<TaskItem>());

        // Act
        var result = await _controller.GetTasksByUserId(99); // Any ID

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<List<TaskItem>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(200, response.StatusCode);
        Assert.Empty(response.Data);
    }
    [Fact]
    public async Task GetTasksByUserId_Returns500_OnException()
    {
        // Arrange
        _taskServiceMock
            .Setup(x => x.GetTasksByUserIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetTasksByUserId(1);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }




    [Fact]
    public async Task DeleteTask_ReturnsOk_WhenTaskDeleted()
    {
        _taskServiceMock.Setup(x => x.DeleteTaskAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteTask(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
        Assert.Equal(ResponseMessages.Task.Deleted, response.Message);
    }


    [Fact]
    public async Task DeleteTask_ReturnsNotFound_WhenTaskNotFound()
    {
        _taskServiceMock.Setup(x => x.DeleteTaskAsync(1)).ReturnsAsync(false);

        var result = await _controller.DeleteTask(1);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFound.Value);
    }

    [Fact]
    public async Task UpdateTask_ReturnsOk_WhenSuccess()
    {
        var updatedTask = new TaskItem { Id = 1, Name = "Updated" };

        _taskServiceMock.Setup(x => x.UpdateTaskAsync(1, updatedTask)).ReturnsAsync(updatedTask);

        var result = await _controller.UpdateTask(1, updatedTask);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task UpdateTask_ReturnsNotFound_WhenTaskNotFound()
    {
        _taskServiceMock.Setup(s => s.UpdateTaskAsync(999, It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem)null);

        var result = await _controller.UpdateTask(999, new TaskItem());

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }



    [Fact]
    public async Task RunDueDateCheck_ReturnsOk()
    {
        _taskStateServiceMock.Setup(x => x.UpdateTaskStatesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.RunDueDateCheck(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }
    [Fact]
    public async Task GetTasks_ReturnsOk_WithPagedResults()
    {
        // Arrange
        var mockTasks = new List<TaskItem>
    {
        new TaskItem { Id = 1, Name = "Test Task 1" },
        new TaskItem { Id = 2, Name = "Test Task 2" }
    };

        var pagedResult = new PagedResult<TaskItem>
        {
            Tasks = mockTasks,
            TotalCount = mockTasks.Count
        };

        var statusCounts = new TaskStatusCount
        {
            Completed = 1,
            Pending = 1,
            New = 0
        };

        _taskServiceMock
     .Setup(s => s.GetTasksAsync(It.IsAny<int>(), It.IsAny<int>()))
     .Returns(Task.FromResult((pagedResult.Tasks, pagedResult.TotalCount)));


        _taskServiceMock.Setup(s => s.GetTaskStatusCountsAsync())
            .ReturnsAsync(statusCounts);

        // Act
        var result = await _controller.GetTasks();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);

        Assert.True(apiResponse.Success);
        Assert.Equal(200, apiResponse.StatusCode);
        Assert.NotNull(apiResponse.Data);

        var json = JsonSerializer.Serialize(apiResponse.Data);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("tasks", out _));
        Assert.Equal(mockTasks.Count, root.GetProperty("totalCount").GetInt32());
        Assert.Equal(statusCounts.Completed, root.GetProperty("completedCount").GetInt32());
        Assert.Equal(statusCounts.Pending, root.GetProperty("pendingCount").GetInt32());
        Assert.Equal(statusCounts.New, root.GetProperty("newCount").GetInt32());
    }

    [Fact]
    public async Task Upload_ReturnsOkResult_WhenParsingSucceeds()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = "Fake file content";
        var fileName = "test.csv";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.ContentType).Returns("text/csv");

        var parseResult = new TaskParseResult
        {
            Success = true,
            ParsedTasks = new List<ParsedTask>
            {
                new ParsedTask { Name = "Task 1" },
                new ParsedTask { Name = "Task 2" }
            }
        };

        _taskUploadServiceMock
            .Setup(s => s.ParseTasksFromFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync((true, null, parseResult.ParsedTasks)); // Adjusted to match the expected tuple return type

        // Act
        var result = await _controller.Upload(fileMock.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(okResult.Value);

        // Serialize response.Data to JSON then deserialize it to UploadResponseData
        var json = JsonSerializer.Serialize(response.Data);
        var parsedData = JsonSerializer.Deserialize<UploadResponseData>(json);

        // Now assert values
        Assert.Equal("Tasks parsed successfully", parsedData.message);
        Assert.Equal(2, parsedData.count);
        Assert.Equal(2, parsedData.data.Count);

    }

    [Fact]
    public async Task Upload_ReturnsBadRequest_WhenParsingFails()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();

        _taskUploadServiceMock
            .Setup(s => s.ParseTasksFromFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync((false, "Parsing failed due to invalid format", null)); 

        // Act
        var result = await _controller.Upload(fileMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);

        Assert.False(response.Success);
        Assert.Equal(400, response.StatusCode);
        Assert.Contains("Parsing failed due to invalid format", response.Errors);
    }



}
