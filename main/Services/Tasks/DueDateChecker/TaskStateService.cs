using AuthService.Data;
using TaskManager.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Services;

namespace TaskManager.Services.Tasks.DueDateChecker
{
    public class TaskStateService : ITaskStateService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TaskStateService> _logger;

        public TaskStateService(AppDbContext dbContext, ILogger<TaskStateService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task UpdateTaskStatesAsync(CancellationToken cancellationToken)
        {
            var now = DateHelper.Today;
            var nextDay = DateHelper.Tomorrow;

            var allTasks = await _dbContext.Tasks.ToListAsync(cancellationToken);

            foreach (var task in allTasks)
            {
                if (task.Duedate.HasValue &&
                    (task.Duedate.Value.Date == now || task.Duedate.Value.Date == nextDay))
                {
                    task.State = TaskStates.Due;
                    _logger.LogInformation($"📌 Task '{task.Name}' is due ({task.Duedate:yyyy-MM-dd}) - State set to 'due'.");
                    continue;
                }

                if (task.Duedate.HasValue && task.Duedate.Value.Date < now)
                {
                    task.State = TaskStates.Overdue;
                    _logger.LogInformation($"⏰ Task '{task.Name}' is overdue - was due on {task.Duedate:yyyy-MM-dd}.");
                    continue;
                }

                //if (!string.IsNullOrEmpty(task.Status))
                //{
                //    task.State = task.Status.ToLower();
                //    _logger.LogInformation($"📝 Task '{task.Name}' - Status '{task.Status}' copied to State.");
                //}
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
