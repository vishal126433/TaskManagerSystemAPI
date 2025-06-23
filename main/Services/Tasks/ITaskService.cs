
using System.Threading.Tasks;
using AuthService.Models;

namespace TaskManager.Services.Tasks
{
    public interface ITaskService
    {
        Task<TaskItem> CreateTaskAsync(int userId, TaskItem task);
        Task<bool> DeleteTaskAsync(int id);
        Task<TaskItem?> UpdateTaskAsync(int id, TaskItem updatedTask);
        Task<List<string>> GetStatusListAsync();
        Task<List<TaskItem>> GetTasksByUserIdAsync(int userId);


    }
}
