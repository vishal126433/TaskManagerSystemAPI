using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using AuthService.Data;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Logging;
using TaskManager.Services;
using TaskManager.Services.Tasks;
using TaskManager.Services.Users;
using TaskManager.Services.Tasks.DueDateChecker;
using TaskManager.Services.Tasks.FileUpload;




IdentityModelEventSource.ShowPII = true;


var builder = WebApplication.CreateBuilder(args);



// Add services to the container
builder.Services.AddControllers();
//builder.Services.AddAuthentication();


builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme,
        securityScheme: new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "JWT Authorization header using the Bearer scheme",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference =new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    new string[] {}
                }
                });
}
);

builder.Services.AddHttpClient<IUserService, UserService>();
builder.Services.AddScoped<ITaskUploadService, TaskUploadService>();
builder.Services.AddHostedService<TaskDueDateChecker>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskStateService, TaskStateService>();
builder.Services.AddScoped<ITaskDataParser, ExcelTaskDataParser>(); // For now only Excel







// Configure Entity Framework with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);


// CORS policy to allow frontend (Angular) app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              //.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("Set-Cookie", "Authorization", "Expires");

    });
});



// JWT Authentication



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            //ClockSkew = TimeSpan.FromMinutes(10),
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
             Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
            RoleClaimType = ClaimTypes.Role
            //RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"


        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("? Authentication failed:");
                Console.WriteLine(context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("? Token validated.");
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();


var app = builder.Build();

app.UseCors("AngularApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    var token = context.Request.Headers["Authorization"].ToString();
    Console.WriteLine($"?? Raw Authorization Header: {token}");
    await next();
});

app.UseRouting();
app.UseAuthentication(); // ? Ensures JWT is validated and User is set
app.UseAuthorization();

app.MapControllers();

app.Run();
