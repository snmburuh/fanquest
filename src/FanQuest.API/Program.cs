using AspNetCoreRateLimit;
using FanQuest.API.Configuration;
using FanQuest.API.Data;
using FanQuest.API.Hubs;
using FanQuest.API.Middleware;
using FanQuest.API.Services;
using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Application.Interfaces.Services;
using FanQuest.Application.UseCases.JoinQuest;
using FanQuest.Domain.Entities;
using FanQuest.Domain.Rules;
using FanQuest.Infrastructure.Caching;
using FanQuest.Infrastructure.Payments.Mpesa;
using FanQuest.Infrastructure.Persistence;
using FanQuest.Infrastructure.Repositories;
using FanQuest.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// ADD THIS: Configure CORS
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",     // React dev server
                "http://localhost:5173",     // Vite dev server (if using Vite)
                "https://yourdomain.com"     // Production frontend URL
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Host.UseSerilog((context, configuration) =>
{

    configuration
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.File(@"C:\FanQuest\API\Logs\" + DateTime.Now.ToString("yyyyMMdd") + @"\FanQuest.Api.log", rollingInterval: RollingInterval.Hour, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{ClientIp}] [{RequestId}] [{RequestPath}] [{Message:lj}] [{Exception}]{NewLine}");
});


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<FanQuestDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FanQuestDb")));

// Redis Configuration
var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnection))
{
    try
    {
        // Register IConnectionMultiplexer as Singleton
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = ConfigurationOptions.Parse(redisConnection);
            configuration.AbortOnConnectFail = false;
            configuration.ConnectTimeout = 5000;
            configuration.SyncTimeout = 5000;
            return ConnectionMultiplexer.Connect(configuration);
        });

        // Register IDistributedCache (optional, for session/cache)
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.ConfigurationOptions = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                ConnectTimeout = 5000
            };
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Redis connection failed: {ex.Message}");
    }
}

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);
builder.Services.AddScoped<JwtTokenService>();

// Authentication
var jwtSecret = jwtSettings.Get<JwtSettings>()?.Secret ?? throw new InvalidOperationException("JWT Secret not configured");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
    ValidateIssuer = true,
    ValidIssuer = jwtSettings.Get<JwtSettings>()?.Issuer,
    ValidateAudience = true,
    ValidAudience = jwtSettings.Get<JwtSettings>()?.Audience,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};




    // SignalR support
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// SignalR
builder.Services.AddSignalR();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQuestRepository, QuestRepository>();
builder.Services.AddScoped<IParticipationRepository, ParticipationRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IRewardRepository, RewardRepository>(); // ← ADDED
builder.Services.AddScoped<IChallengeRepository, ChallengeRepository>();
// Services
builder.Services.AddScoped<ILeaderboardService, RedisLeaderboardService>();
// M-Pesa Services
builder.Services.AddHttpClient<IMpesaClient, MpesaClient>();
builder.Services.AddScoped<IMpesaConfigService, MpesaConfigService>();
builder.Services.AddScoped<IPaymentService, MpesaPaymentService>();
// Add after existing repository registrations
builder.Services.AddScoped<ITenantRepository, TenantRepository>();

// Add after existing service registrations
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();


builder.Services.AddSingleton<QuestNotificationService>();
builder.Services.AddScoped<JwtTokenService>();

// Challenge Rules
builder.Services.AddScoped<IChallengeRule, TimeWindowRule>();
builder.Services.AddScoped<IChallengeRule>(sp =>
{
    var repo = sp.GetRequiredService<IParticipationRepository>();
    return new SingleCompletionRule((userId, challengeId) =>
        repo.HasCompletedChallengeAsync(userId, challengeId).Result);
});
builder.Services.AddScoped<IChallengeRule, LocationRule>();
builder.Services.AddScoped<ChallengeRuleEngine>();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(JoinQuestHandler).Assembly));

// CORS for Mini App
builder.Services.AddCors(options =>
{
    options.AddPolicy("MiniAppPolicy", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "*" })
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// M-Pesa Configuration
builder.Services.Configure<MpesaConfiguration>(builder.Configuration.GetSection("Mpesa"));
builder.Services.AddHostedService<QuestCompletionService>();

// Add Swagger/OpenAPI - ADD THIS SECTION
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "FanQuest API",
        Description = "Gamified fan engagement platform API for M-Pesa Mini App",
        Contact = new OpenApiContact
        {
            Name = "FanQuest Team"
        }
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddRateLimiting(builder.Configuration);
// Register Application Layer Services
builder.Services.AddScoped<JoinQuestHandler>();
var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;

    try
    {
        FanQuestDbContext context = scope.ServiceProvider.GetRequiredService<FanQuestDbContext>();
        if (context.Database.IsSqlServer()) { context.Database.Migrate(); }

        await SeedData.Initialize(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        Log.Error($"FanQuest.Api => Program: An error while setting up infrastructure - migration, sequences and seed {ex.Message}");
        throw;
    }
}


// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FanQuest API v1");
        options.RoutePrefix = string.Empty; // Makes Swagger UI the root page
    });
}
app.UseCors("AllowFrontend");
app.UseIpRateLimiting();
app.UseMiddleware<CustomRateLimitMiddleware>();
// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();
app.MapHub<QuestHub>("/hubs/quest");

app.Run();
