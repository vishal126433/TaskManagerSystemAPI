using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using AuthService.Data;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Logging;
using System.IdentityModel.Tokens.Jwt;

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
            //RoleClaimType = ClaimTypes.Role
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"


        };

        //  Add this block to log why token validation fails
        //options.Events = new JwtBearerEvents
        //{
        //    // ?? This runs before token validation
        //    OnMessageReceived = context =>
        //    {
        //        var authHeader = context.Request.Headers["Authorization"].ToString();

        //        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        //        {
        //            var token = authHeader.Substring("Bearer ".Length).Trim().Trim('"');
        //            context.Token = token; // ? This overrides the token used in validation
        //        }

        //        return Task.CompletedTask;
        //    },

        //    OnAuthenticationFailed = context =>
        //    {
        //        Console.WriteLine("? Authentication failed:");
        //        Console.WriteLine(context.Exception.Message);

        //        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        //        var configuredIssuer = jwtSettings["Issuer"];
        //        var configuredAudience = jwtSettings["Audience"];
        //        var configuredKey = jwtSettings["SecretKey"];

        //        var authHeader = context.Request.Headers["Authorization"].ToString();

        //        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        //        {
        //            var tokenStr = authHeader.Substring("Bearer ".Length).Trim('"');
        //            Console.WriteLine("?? Token that failed: " + tokenStr);

        //            try
        //            {
        //                // Decode the token to inspect values
        //                var handler = new JwtSecurityTokenHandler();
        //                var jwtToken = handler.ReadJwtToken(tokenStr);

        //                Console.WriteLine("\n--- TOKEN VS CONFIG COMPARISON ---");

        //                // ?? Issuer
        //                Console.WriteLine("?? Token Issuer (iss): " + jwtToken.Issuer);
        //                Console.WriteLine("??? Configured Issuer: " + configuredIssuer);

        //                // ?? Audience
        //                var tokenAudience = jwtToken.Audiences.FirstOrDefault();
        //                Console.WriteLine("?? Token Audience (aud): " + tokenAudience);
        //                Console.WriteLine("??? Configured Audience: " + configuredAudience);


        //                // ?? Expiration
        //                Console.WriteLine("?? Token Expiry (exp): " + jwtToken.ValidTo + " (UTC)");
        //                Console.WriteLine("?? Current Time: " + DateTime.UtcNow);

        //                // ?? Algorithm
        //                Console.WriteLine("?? Token Alg: " + jwtToken.Header.Alg);
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine("?? Failed to parse JWT token: " + ex.Message);
        //            }
        //        }

        //        return Task.CompletedTask;
        //    },

        //    OnTokenValidated = context =>
        //    {
        //        Console.WriteLine("? JWT validated successfully.");
        //        return Task.CompletedTask;
        //    }
        //};
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
