
namespace TaskManager.Services.Tasks.FileUpload
{
    public interface ITaskDataParser
    {
        Task<List<TaskItem>> ParseAsync(IFormFile file);
    }
}