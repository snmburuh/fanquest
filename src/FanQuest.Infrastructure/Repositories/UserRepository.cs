using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Domain.Entities;
using FanQuest.Domain.Enums;
using global::FanQuest.Application.Interfaces.Repositories;
using global::FanQuest.Domain.Entities;
using global::FanQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FanQuest.Infrastructure.Repositories
{
    

    public class UserRepository : IUserRepository
    {
        private readonly FanQuestDbContext _context;

        public UserRepository(FanQuestDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }

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

    public class PaymentRepository : IPaymentRepository
    {
        private readonly FanQuestDbContext _context;

        public PaymentRepository(FanQuestDbContext context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _context.Payments.FindAsync(id);
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}
