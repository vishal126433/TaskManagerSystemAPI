using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using TaskManager.Extensions;
using TaskManager.Services.Tasks;
using TaskManager.Services.Users;
using TaskManager.Services.Tasks.DueDateChecker;
using TaskManager.Services.Tasks.FileUpload;
using TaskManager.Helpers;
using TaskManager.Services.Notifications;
using TaskManagerSystemAPI.Middlewares;
using AuthService.Data;

IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Use clean extension methods
builder.Services.AddCustomSwagger();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAngularCors();

// Register other services
builder.Services.AddHttpClient<IUserService, UserService>();
builder.Services.AddScoped<ITaskUploadService, TaskUploadService>();
builder.Services.AddHostedService<TaskDueDateChecker>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskStateService, TaskStateService>();
builder.Services.AddScoped<ITaskDataParser, ExcelTaskDataParser>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<TaskNotificationSettings>(
    builder.Configuration.GetSection("TaskNotificationSettings"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

app.UseCors("AngularApp");

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
