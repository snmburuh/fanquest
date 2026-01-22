using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Domain.Entities;
using FanQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FanQuest.Infrastructure.Repositories
{
    public class RewardRepository : IRewardRepository
    {
        private readonly FanQuestDbContext _context;

        public RewardRepository(FanQuestDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reward>> GetRewardsByQuestIdAsync(Guid questId)
        {
            return await _context.Rewards
                .Where(r => r.QuestId == questId)
                .OrderBy(r => r.MinRank)
                .ToListAsync();
        }

        public async Task<Reward?> GetByIdAsync(Guid id)
        {
            return await _context.Rewards.FindAsync(id);
        }

        public async Task<Reward> CreateAsync(Reward reward)
        {
            _context.Rewards.Add(reward);
            await _context.SaveChangesAsync();
            return reward;
        }
    }
}
