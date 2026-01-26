using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Domain.Entities;
using FanQuest.Infrastructure.Persistence;

namespace FanQuest.Infrastructure.Repositories
{
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

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public Task<Payment> GetByCheckoutRequestIdAsync(string checkoutRequestId)
        {
            throw new NotImplementedException();
        }

        public Task<Payment> GetByConversationIdAsync(string conversationId)
        {
            throw new NotImplementedException();
        }
    }
}
