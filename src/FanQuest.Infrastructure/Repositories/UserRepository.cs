using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Domain.Entities;
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
    
}
