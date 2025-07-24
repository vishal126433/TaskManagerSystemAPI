using Microsoft.AspNetCore.Http;
using TaskManager.Services.FileUpload.Models;

namespace TaskManager.Services.FileUpload.FileUploads
{
    public class TaskUploadService : ITaskUploadService
    {
        private readonly ITaskDataParser _parser;

        public TaskUploadService(ITaskDataParser parser)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public async Task<(bool Success, string ErrorMessage, List<ParsedTask> ParsedTasks)> ParseTasksFromFileAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return (false, "No file uploaded.", null);

                var extension = Path.GetExtension(file.FileName).ToLower();

                if (extension == ".xlsx" || extension == ".xls")
                {
                    var parsedTasks = await _parser.ParseAsync(file); // Should return List<ParsedTask>
                    return (true, "", parsedTasks);
                }
                else if (extension == ".csv")
                {
                    return (false, "CSV parsing not implemented.", null);
                }
                else
                {
                    return (false, "Unsupported file type.", null);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

    }
}
