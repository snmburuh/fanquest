using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Domain.Entities;
using FanQuest.Infrastructure.Persistence;

namespace FanQuest.Infrastructure.Repositories
{
    public class ChallengeRepository : IChallengeRepository
    {
        private readonly FanQuestDbContext _context;

        public ChallengeRepository(FanQuestDbContext context)
        {
            _context = context;
        }

        public async Task<Challenge> CreateAsync(Challenge challenge)
        {
            _context.Challenges.Add(challenge);
            await _context.SaveChangesAsync();
            return challenge;
        }

        public async Task<Challenge?> GetByIdAsync(Guid id)
        {
            return await _context.Challenges.FindAsync(id);
        }
    }
}
