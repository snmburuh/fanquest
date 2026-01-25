using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Domain.Entities;

namespace FanQuest.API.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var questRepo = serviceProvider.GetRequiredService<IQuestRepository>();
            var rewardRepo = serviceProvider.GetRequiredService<IRewardRepository>();

            // Check if data already exists
            var existingQuests = await questRepo.GetActiveQuestsAsync();
            if (existingQuests.Any())
            {
                Console.WriteLine("Database already seeded");
                return;
            }

            // Create sample quest
            var quest = new Quest(
                "Speed Nairobi Quest",
                "Nairobi",
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(3),
                20);

            quest.Publish();

            // Add challenges
            var checkInChallenge = new CheckInChallenge(
                quest.Id,
                "Check-in at Venue",
                30,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(2),
                "KICC, Nairobi CBD");

            var timedChallenge = new TimedChallenge(
                quest.Id,
                "Early Bird Bonus",
                20,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(30));

            var reactionChallenge = new ReactionChallenge(
                quest.Id,
                "Live Event Reaction",
                25,
                DateTime.UtcNow.AddMinutes(45),
                DateTime.UtcNow.AddHours(1));

            quest.AddChallenge(checkInChallenge);
            quest.AddChallenge(timedChallenge);
            quest.AddChallenge(reactionChallenge);

            await questRepo.CreateAsync(quest);

            // Add rewards
            var rewards = new[]
            {
            new Reward(quest.Id, "Top 3 Reward", 500, 1, 3),
            new Reward(quest.Id, "Top 10 Reward", 200, 4, 10),
            new Reward(quest.Id, "Participation Reward", 50, 11, 50)
        };

            foreach (var reward in rewards)
            {
                await rewardRepo.CreateAsync(reward);
            }

            Console.WriteLine("Sample quest seeded successfully!");
            Console.WriteLine($"Quest ID: {quest.Id}");
            Console.WriteLine($"Challenges: {quest.Challenges.Count}");
            Console.WriteLine($"Rewards: {rewards.Length}");
        }
    }
}
