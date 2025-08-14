using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using TaskManager.Extensions;
//using TaskManager.Services.Tasks;
//using TaskManager.Services.Users;
using TaskManager.Interfaces;
using TaskManager.Services;
//using TaskManager.Services.Tasks.DueDateChecker;
using TaskManager.Helpers;
//using TaskManager.Services.Notifications;
using TaskManagerSystemAPI.Middlewares;
using TaskManager.Data;
using Serilog;
using Serilog.Events;
//using TaskManager.Services.Tasks.FileUpload;
//using TaskManager.Services.FileUpload.FileUploads;
using TaskManager.Services.FileUpload.Factories;
using TaskManager.Services.FileUpload.Interfaces;
using TaskManager.Services.FileUpload.Services;
using TaskManager.Services.FileUpload.Parsers;





IdentityModelEventSource.ShowPII = true;
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // overall level
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAngularCors(builder.Configuration);


// Use clean extension methods
builder.Services.AddCustomSwagger();
builder.Services.AddJwtAuthentication(builder.Configuration);

// Register other services
builder.Services.AddHttpClient<IUserService, UserService>();
builder.Services.AddScoped<ITaskUploadService, TaskUploadService>();

builder.Services.AddScoped<IParserFactory, ParserFactory>();


//builder.Services.AddHostedService<TaskDueDateChecker>();

builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddScoped<ITaskStateService, TaskStateService>();
builder.Services.AddScoped<ITaskDataParser, ExcelTaskDataParser>();
builder.Services.AddScoped<ExcelTaskDataParser>(); // ? Explicit registration


builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<TaskNotificationSettings>(
    builder.Configuration.GetSection("TaskNotificationSettings"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();
var corsPolicyName = builder.Configuration.GetValue<string>("Cors:PolicyName");

app.UseCors(corsPolicyName);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
