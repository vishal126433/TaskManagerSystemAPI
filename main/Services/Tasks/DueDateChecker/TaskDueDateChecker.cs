using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.Helpers;
using TaskManager.Services;

namespace TaskManager.Services.Tasks.DueDateChecker
{
    public class TaskDueDateChecker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TaskDueDateChecker> _logger;
        private readonly TaskNotificationSettings _settings;


        public TaskDueDateChecker(IServiceProvider serviceProvider, ILogger<TaskDueDateChecker> logger, IOptions<TaskNotificationSettings> settings)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = settings.Value;

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

                await Task.Delay(TimeSpan.FromMinutes(_settings.TaskCheckIntervalMinutes), stoppingToken);
            }
        }
    }

}
