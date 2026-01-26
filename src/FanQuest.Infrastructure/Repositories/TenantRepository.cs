using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Domain.Entities;
using FanQuest.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FanQuest.Infrastructure.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly FanQuestDbContext _context;

        public TenantRepository(FanQuestDbContext context)
        {
            _context = context;
        }

        public async Task<Tenant?> GetByIdAsync(Guid tenantId)
        {
            return await _context.Tenants
                .Include(t => t.MpesaConfiguration)
                .FirstOrDefaultAsync(t => t.Id == tenantId);
        }

        public async Task<Tenant?> GetByApiKeyAsync(string apiKey)
        {
            return await _context.Tenants
                .Include(t => t.MpesaConfiguration)
                .FirstOrDefaultAsync(t => t.ApiKey == apiKey && t.IsActive);
        }

        public async Task<IEnumerable<Tenant>> GetAllActiveAsync()
        {
            return await _context.Tenants
                .Include(t => t.MpesaConfiguration)
                .Where(t => t.IsActive)
                .ToListAsync();
        }

        public async Task AddAsync(Tenant tenant)
        {
            await _context.Tenants.AddAsync(tenant);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Tenant tenant)
        {
            _context.Tenants.Update(tenant);
            await _context.SaveChangesAsync();
        }
    }
}
