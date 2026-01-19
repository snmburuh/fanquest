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

// Redis
var redisConnection = ConnectionMultiplexer.Connect(
    builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);

// SignalR
builder.Services.AddSignalR();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQuestRepository, QuestRepository>();
builder.Services.AddScoped<IParticipationRepository, ParticipationRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

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
