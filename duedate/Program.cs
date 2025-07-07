using duedate;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateApplicationBuilder(args);

// Add configuration (to get API base URL, interval etc.)
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add TaskNotificationSettings config binding
builder.Services.Configure<TaskNotificationSettings>(
    builder.Configuration.GetSection("TaskNotificationSettings"));

// Register HttpClient to call the API
builder.Services.AddHttpClient();

// Add Worker service
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
