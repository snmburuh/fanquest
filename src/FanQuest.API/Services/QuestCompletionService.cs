using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Domain.Enums;

namespace FanQuest.API.Services
{
    public class QuestCompletionService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QuestCompletionService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public QuestCompletionService(
            IServiceProvider serviceProvider,
            ILogger<QuestCompletionService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Quest Completion Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndCompleteExpiredQuests();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Quest Completion Service");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Quest Completion Service stopped");
        }

        private async Task CheckAndCompleteExpiredQuests()
        {
            using var scope = _serviceProvider.CreateScope();
            var questRepo = scope.ServiceProvider.GetRequiredService<IQuestRepository>();

            var activeQuests = await questRepo.GetActiveQuestsAsync();
            var expiredQuests = activeQuests.Where(q => q.EndsAt <= DateTime.UtcNow && q.Status == QuestStatus.Live);

            foreach (var quest in expiredQuests)
            {
                _logger.LogInformation("Auto-completing expired quest: {QuestId} - {Name}", quest.Id, quest.Name);

                quest.Complete();
                await questRepo.UpdateAsync(quest);

                _logger.LogInformation("Quest completed: {QuestId}", quest.Id);
            }

            if (expiredQuests.Any())
            {
                _logger.LogInformation("Completed {Count} expired quests", expiredQuests.Count());
            }
        }
    }
}
