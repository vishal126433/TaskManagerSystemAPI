using duedate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
    .UseWindowsService() // makes it run properly as a Windows Service
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory); // ensures correct folder even as service
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<TaskNotificationSettings>(
            hostContext.Configuration.GetSection("TaskNotificationSettings"));
        services.AddHttpClient();
        services.AddHostedService<Worker>();
    })
    .Build()
    .Run();
