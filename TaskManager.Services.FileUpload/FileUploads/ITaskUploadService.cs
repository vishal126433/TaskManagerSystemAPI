using Microsoft.AspNetCore.Http;
using TaskManager.Services.FileUpload.Models;

namespace TaskManager.Services.FileUpload.FileUploads
{
    public interface ITaskUploadService
    {
        
        Task<(bool Success, string ErrorMessage, List<ParsedTask> ParsedTasks)> ParseTasksFromFileAsync(IFormFile file);
    }
}
