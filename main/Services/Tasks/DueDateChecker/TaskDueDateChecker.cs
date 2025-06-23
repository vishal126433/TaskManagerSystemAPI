using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Services;

namespace TaskManager.Services.Tasks.DueDateChecker
{
    public class TaskDueDateChecker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TaskDueDateChecker> _logger;

        public TaskDueDateChecker(IServiceProvider serviceProvider, ILogger<TaskDueDateChecker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var taskService = scope.ServiceProvider.GetRequiredService<ITaskStateService>();
                    await taskService.UpdateTaskStatesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error running background task.");
                }

                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
            }
        }
    }

}
