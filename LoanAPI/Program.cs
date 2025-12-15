using LoanAPI.Data;
using LoanAPI.Services.Auth;
using LoanAPI.Services.Loan;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Serilog;
using FluentValidation;  

// SeriLog Configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-. txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Loan API...");

    var builder = WebApplication.CreateBuilder(args);

    // SeriLog
    builder.Host.UseSerilog();

    var configuration = builder.Configuration;

    // Database
    builder.Services.AddDbContext<LoanDbContext>(opt =>
        opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

    // Controllers
    builder.Services.AddControllers();  

    // FluentValidation
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();  

    builder.Services.AddEndpointsApiExplorer();

    // Swagger
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Loan API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization.  Example: \"Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType. SecurityScheme, Id = "Bearer" } },
                new string[] {}
            }
        });
    });

    // Services Registration
    builder.Services.AddScoped<SAuthService, AuthService>();
    builder.Services.AddScoped<SLoanService, LoanService>();

    // JWT Authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
            };
        });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    // SeriLog Request Logging
    app.UseSerilogRequestLogging();

    // Exception Handling
    app.Use(async (context, next) =>
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception occurred");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { message = "Internal server error", error = ex.Message });
        }
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();  
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Loan API started successfully!");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}