using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Domain.Entities;
using FanQuest.Domain.Enums;
using FanQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FanQuest.Infrastructure.Repositories
{
    public class QuestRepository : IQuestRepository
    {
        private readonly FanQuestDbContext _context;

        public QuestRepository(FanQuestDbContext context)
        {
            _context = context;
        }

        public async Task<Quest?> GetByIdAsync(Guid id)
        {
            return await _context.Quests
                .Include(q => q.Challenges)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Quest>> GetActiveQuestsAsync(string? city = null)
        {
            var query = _context.Quests
                .Include(q => q.Challenges)
                .Where(q => q.Status == QuestStatus.Live && q.EndsAt > DateTime.UtcNow);

            if (!string.IsNullOrEmpty(city))
                query = query.Where(q => q.City == city);

            return await query.ToListAsync();
        }

        public async Task<Quest> CreateAsync(Quest quest)
        {
            _context.Quests.Add(quest);
            await _context.SaveChangesAsync();
            return quest;
        }

        public async Task UpdateAsync(Quest quest)
        {
            _context.Quests.Update(quest);
            await _context.SaveChangesAsync();
        }
    }
}
