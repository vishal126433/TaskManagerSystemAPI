using AuthService.Data;
using System.IO;

namespace TaskManager.Services.Tasks.FileUpload
{
    public class TaskUploadService : ITaskUploadService
    {
        private readonly AppDbContext _db;
        private readonly ITaskDataParser _parser;
        private readonly ITaskService _taskService;


        public TaskUploadService(AppDbContext db, ITaskDataParser parser, ITaskService taskService)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db), "DbContext cannot be null.");
            _parser = parser ?? throw new ArgumentNullException(nameof(parser), "parser cannot be null.");
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService), "taskService cannot be null.");

        }

        public async Task<(bool Success, string ErrorMessage, int Count)> ProcessExcelAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return (false, "No file uploaded.", 0);

                var extension = Path.GetExtension(file.FileName).ToLower();

                if (extension == ".xlsx" || extension == ".xls")
                {
                    var parsedTasks = await _parser.ParseAsync(file);
                    int createdCount = 0;

                    foreach (var parsedTask in parsedTasks)
                    {
                        // call CreateTaskAsync to validate & set state before saving
                        await _taskService.CreateTaskAsync(parsedTask.UserId, parsedTask);
                        createdCount++;
                    }
                    return (true, "", createdCount);
                }
                else if (extension == ".csv")
                {
                    return (false, "CSV parsing not implemented.", 0);
                }
                else
                {
                    return (false, "Unsupported file type.", 0);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, 0);
            }
        }
    }
}
