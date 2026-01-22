using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Domain.Entities;
using FanQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Infrastructure.Repositories
{
    public class ParticipationRepository : IParticipationRepository
    {
        private readonly FanQuestDbContext _context;

        public ParticipationRepository(FanQuestDbContext context)
        {
            _context = context;
        }

        public async Task<Participation?> GetByIdAsync(Guid id)
        {
            return await _context.Participations
                .Include(p => p.Completions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Participation?> GetByUserAndQuestAsync(Guid userId, Guid questId)
        {
            return await _context.Participations
                .Include(p => p.Completions)
                .FirstOrDefaultAsync(p => p.UserId == userId && p.QuestId == questId);
        }

        public async Task<bool> HasCompletedChallengeAsync(Guid userId, Guid challengeId)
        {
            return await _context.Completions
                .AnyAsync(c => c.ParticipationId ==
                    _context.Participations
                        .Where(p => p.UserId == userId)
                        .Select(p => p.Id)
                        .FirstOrDefault()
                    && c.ChallengeId == challengeId);
        }

        public async Task<Participation> CreateAsync(Participation participation)
        {
            _context.Participations.Add(participation);
            await _context.SaveChangesAsync();
            return participation;
        }

        public async Task UpdateAsync(Participation participation)
        {
            _context.Participations.Update(participation);
            await _context.SaveChangesAsync();
        }
    }
}
