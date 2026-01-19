using FanQuest.Application.DTOs;
using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Infrastructure.Caching
{
    public class RedisLeaderboardService : ILeaderboardService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<RedisLeaderboardService> _logger;

        public RedisLeaderboardService(
            IConnectionMultiplexer redis,
            IUserRepository userRepo,
            ILogger<RedisLeaderboardService> logger)
        {
            _redis = redis;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync(Guid questId, int limit = 50)
        {
            var db = _redis.GetDatabase();
            var key = GetLeaderboardKey(questId);

            var entries = await db.SortedSetRangeByRankWithScoresAsync(
                key,
                0,
                limit - 1,
                Order.Descending);

            var results = new List<LeaderboardEntryDto>();
            int rank = 1;

            foreach (var entry in entries)
            {
                var userId = Guid.Parse(entry.Element.ToString());
                var user = await _userRepo.GetByIdAsync(userId);

                if (user != null)
                {
                    results.Add(new LeaderboardEntryDto
                    {
                        Rank = rank++,
                        UserId = userId,
                        DisplayName = user.DisplayName,
                        Score = (int)entry.Score
                    });
                }
            }

            _logger.LogInformation("Retrieved {Count} leaderboard entries for quest {QuestId}",
                results.Count, questId);

            return results;
        }

        public async Task UpdateScoreAsync(Guid questId, Guid userId, int score)
        {
            var db = _redis.GetDatabase();
            var key = GetLeaderboardKey(questId);

            await db.SortedSetAddAsync(key, userId.ToString(), score);

            _logger.LogInformation("Updated score for user {UserId} in quest {QuestId}: {Score}",
                userId, questId, score);
        }

        public async Task<int?> GetUserRankAsync(Guid questId, Guid userId)
        {
            var db = _redis.GetDatabase();
            var key = GetLeaderboardKey(questId);

            var rank = await db.SortedSetRankAsync(key, userId.ToString(), Order.Descending);

            return rank.HasValue ? (int)rank.Value + 1 : null;
        }

        private static string GetLeaderboardKey(Guid questId) => $"leaderboard:{questId}";
    }
}
