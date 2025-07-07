using Microsoft.Extensions.Options;
using System.Net.Http;

namespace duedate
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TaskNotificationSettings _settings;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory, IOptions<TaskNotificationSettings> settings, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var apiBaseUrl = _configuration.GetValue<string>("ApiBaseUrl");
            if (string.IsNullOrEmpty(apiBaseUrl))
            {
                _logger.LogError("API base URL is not configured!");
                return;
            }

            var client = _httpClientFactory.CreateClient();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var response = await client.PostAsync($"{apiBaseUrl}/DueDateChecker/run", null, stoppingToken);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Successfully called API to update task states.");
                    }
                    else
                    {
                        _logger.LogWarning("API call failed with status: {StatusCode}", response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calling API");
                }

                await Task.Delay(TimeSpan.FromMinutes(_settings.TaskCheckIntervalMinutes), stoppingToken);
            }
        }
    }
}
