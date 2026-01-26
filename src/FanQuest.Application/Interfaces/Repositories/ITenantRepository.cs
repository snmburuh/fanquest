using FanQuest.Domain.Entities;

namespace FanQuest.Application.Interfaces.Repositories
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetByIdAsync(Guid tenantId);
        Task<Tenant?> GetByApiKeyAsync(string apiKey);
        Task<IEnumerable<Tenant>> GetAllActiveAsync();
        Task AddAsync(Tenant tenant);
        Task UpdateAsync(Tenant tenant);
    }
}
