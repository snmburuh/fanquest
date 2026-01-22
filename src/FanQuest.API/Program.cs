using FanQuest.API.Hubs;
using FanQuest.API.Services;
using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Application.Interfaces.Services;
using FanQuest.Application.UseCases.ClaimReward;
using FanQuest.Application.UseCases.CompleteChallenge;
using FanQuest.Application.UseCases.JoinQuest;
using FanQuest.Domain.Rules;
using FanQuest.Infrastructure.Caching;
using FanQuest.Infrastructure.Payments.Mpesa;
using FanQuest.Infrastructure.Persistence;
using FanQuest.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

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



// SignalR
builder.Services.AddSignalR();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQuestRepository, QuestRepository>();
builder.Services.AddScoped<IParticipationRepository, ParticipationRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IRewardRepository, RewardRepository>(); // ← ADDED

// Services
builder.Services.AddScoped<ILeaderboardService, RedisLeaderboardService>();
builder.Services.AddScoped<IPaymentService, MpesaPaymentService>();

builder.Services.AddScoped<QuestNotificationService>();

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

// Use Cases
builder.Services.AddScoped<JoinQuestHandler>();
builder.Services.AddScoped<CompleteChallengeHandler>();
builder.Services.AddScoped<ClaimRewardHandler>();

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
builder.Services.Configure<MpesaConfig>(builder.Configuration.GetSection("Mpesa"));

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;

    try
    {
        FanQuestDbContext context = scope.ServiceProvider.GetRequiredService<FanQuestDbContext>();
        if (context.Database.IsSqlServer()) { context.Database.Migrate(); }
    }
    catch (Exception ex)
    {
        Log.Error($"FanQuest.Api => Program: An error while setting up infrastructure - migration, sequences and seed {ex.Message}");
        throw;
    }
}


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("MiniAppPolicy");
app.UseAuthorization();

app.MapControllers();
app.MapHub<QuestHub>("/hubs/quest");

app.Run();
