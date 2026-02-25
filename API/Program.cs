using API.Common.Middleware;
using API.Common.Serialization;
using API.Mappings;
using Application.Auth.Interfaces;
using Application.Auth.Services;
using Application.Common;
using Application.Repositories;
using Application.Tasks.Interfaces;
using Application.Tasks.Services;
using Infrastructure;
using Infrastructure.Auth.Options;
using Infrastructure.Auth.Services;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Abstractions;
using Infrastructure.Repositories.EFRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped(typeof(IEFRepository<>), typeof(EFRepository<>));

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddAutoMapper(typeof(TaskMappingProfile).Assembly);

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services.Configure<TokenHashOptions>(
    builder.Configuration.GetSection(TokenHashOptions.SectionName));

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(
            ApiResponse<bool>.FailureResponse("Too many requests"),
            token
        );
    };

    options.AddPolicy("auth", httpContext =>
    {
        var env = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();

        var key = env.IsEnvironment("Testing")
              ? httpContext.Request.Headers["x-test-ratelimit-key"].ToString()
              : (httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip");

        if (string.IsNullOrWhiteSpace(key))
            key = "missing-key";

        var permit = env.IsEnvironment("Testing") ? 1 : 5;
        var window = env.IsEnvironment("Testing") ? TimeSpan.FromMinutes(1) : TimeSpan.FromMinutes(1);

        return RateLimitPartition.GetFixedWindowLimiter(
            key,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permit,
                Window = window,
                QueueLimit = 0
            });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
