using FanQuest.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FanQuest.API.Services
{
    public class QuestNotificationService
    {
        private readonly IHubContext<QuestHub> _hubContext;
        private readonly ILogger<QuestNotificationService> _logger;

        public QuestNotificationService(
            IHubContext<QuestHub> hubContext,
            ILogger<QuestNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyLeaderboardUpdate(Guid questId, object leaderboardData)
        {
            var group = $"quest_{questId}";
            await _hubContext.Clients.Group(group).SendAsync("LeaderboardUpdated", leaderboardData);

            _logger.LogInformation("Leaderboard update sent to quest {QuestId}", questId);
        }

        public async Task NotifyChallengeOpened(Guid questId, object challengeData)
        {
            var group = $"quest_{questId}";
            await _hubContext.Clients.Group(group).SendAsync("ChallengeOpened", challengeData);

            _logger.LogInformation("Challenge opened notification sent to quest {QuestId}", questId);
        }

        public async Task NotifyQuestEnded(Guid questId)
        {
            var group = $"quest_{questId}";
            await _hubContext.Clients.Group(group).SendAsync("QuestEnded", new { questId });

            _logger.LogInformation("Quest ended notification sent to quest {QuestId}", questId);
        }
    }
}
