using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;
using TaskManager.Data;
using TaskManager.Services;
using TaskManager.Interfaces;
using TaskManager.Services.FileUpload.Interfaces;
using TaskManager.DTOs;
using TaskManager.Helpers;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route(ApiTaskEndPoint.BaseApi)]
    public class TasksController : ControllerBase
    {
        private readonly ITaskUploadService _taskUploadService;
        private readonly ITaskService _taskService;
        private readonly ITaskStateService _taskStateService;
        private readonly IUserService _userService;

        public TasksController(ITaskUploadService taskUploadService, ITaskService taskService, ITaskStateService taskStateService, IUserService userService)
        {
            _taskUploadService = taskUploadService ?? throw new ArgumentNullException(nameof(taskUploadService));
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _taskStateService = taskStateService ?? throw new ArgumentNullException(nameof(taskStateService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost(ApiTaskEndPoint.Create)]
        public async Task<IActionResult> CreateTask(int userId, [FromBody] TaskItem task)
        {
            var createdTask = await _taskService.CreateTaskAsync(userId, task);
            return Ok(ApiResponse<object>.SuccessResponse(createdTask, 200, ResponseMessages.Message.TaskCreated));
        }

        [HttpGet(ApiTaskEndPoint.StatusList)]
        public async Task<IActionResult> GetStatusList()
        {
            var statusList = await _taskService.GetStatusListAsync();
            return Ok(ApiResponse<object>.SuccessResponse(statusList));
        }

        [HttpGet(ApiTaskEndPoint.PriorityList)]
        public async Task<IActionResult> GetPriorityList()
        {
            var priorityList = await _taskService.GetPriorityListAsync();
            return Ok(ApiResponse<object>.SuccessResponse(priorityList));
        }

        [HttpGet(ApiTaskEndPoint.TypeList)]
        public async Task<IActionResult> GetTypeList()
        {
            var typeList = await _taskService.GetTypeListAsync();
            return Ok(ApiResponse<object>.SuccessResponse(typeList));
        }

        [HttpGet(ApiTaskEndPoint.Search)]
        public async Task<IActionResult> SearchTasks(int userId, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(ApiResponse<object>.SingleError(ResponseMessages.Message.EmptySearchQuery));

            var tasks = await _taskService.SearchTasksByUserAsync(userId, query);
            return Ok(ApiResponse<object>.SuccessResponse(tasks));
        }

        [HttpGet(ApiTaskEndPoint.SearchTasks)]
        public async Task<IActionResult> SearchTasks(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(ApiResponse<object>.SingleError(ResponseMessages.Message.EmptySearchQuery));

            var tasks = await _taskService.SearchTasksAsync(query);
            return Ok(ApiResponse<object>.SuccessResponse(tasks));
        }

        [HttpPost(ApiTaskEndPoint.UploadJson)]
        public async Task<IActionResult> UploadJson([FromBody] List<TaskImport> tasks)
        {
            if (tasks == null || !tasks.Any())
                return BadRequest(ApiResponse<object>.SingleError(ResponseMessages.Message.EmptyTask));

            var result = await _taskService.UploadTasksAsync(tasks);

            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        [HttpPost(ApiTaskEndPoint.Upload)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var parsedTasks = await _taskUploadService.ParseTasksFromFileAsync(file);
            return Ok(parsedTasks);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetTasksByUserId(int userId)
        {
            var tasks = await _taskService.GetTasksByUserIdAsync(userId);
            return Ok(ApiResponse<List<TaskItem>>.SuccessResponse(tasks));
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks(int pageNumber = 1, int pageSize = 5)
        {
            var result = await _taskService.GetTasksAsync(pageNumber, pageSize);
            var counts = await _taskService.GetTaskStatusCountsAsync();
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                tasks = result.Tasks,
                totalCount = result.TotalCount,
                completedCount = counts.Completed,
                pendingCount = counts.Pending,
                newCount = counts.New
            }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var deleted = await _taskService.DeleteTaskAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.SingleError(ResponseMessages.Message.EmptyTask));

            return Ok(ApiResponse<object>.SuccessResponse(null, 200, ResponseMessages.Message.TaskDeleted));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem updatedTask)
        {
            var result = await _taskService.UpdateTaskAsync(id, updatedTask);
            if (result == null)
                return NotFound(ApiResponse<object>.SingleError(ResponseMessages.Message.EmptyTask));

            return Ok(ApiResponse<object>.SuccessResponse(result, 200, ResponseMessages.Message.TaskUpdated));
        }

        [HttpPost(ApiTaskEndPoint.Run)]
        public async Task<IActionResult> RunDueDateCheck(CancellationToken cancellationToken)
        {
            await _taskStateService.UpdateTaskStatesAsync(cancellationToken);
            return Ok(ApiResponse<object>.SuccessResponse(null, 200, ResponseMessages.Message.Run));
        }
    }
}
