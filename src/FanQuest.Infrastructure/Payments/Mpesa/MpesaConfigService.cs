using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Domain.Entities;

namespace FanQuest.Infrastructure.Payments.Mpesa
{
    public class MpesaConfigService : IMpesaConfigService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IEncryptionService _encryptionService;

        public MpesaConfigService(
            ITenantRepository tenantRepository,
            IEncryptionService encryptionService)
        {
            _tenantRepository = tenantRepository;
            _encryptionService = encryptionService;
        }

        public async Task<MpesaConfiguration> GetConfigurationAsync(Guid tenantId)
        {
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);

            if (tenant?.MpesaConfiguration == null)
                throw new InvalidOperationException($"M-Pesa configuration not found for tenant {tenantId}");

            return tenant.MpesaConfiguration;
        }

        public async Task<string> GetDecryptedConsumerKeyAsync(Guid tenantId)
        {
            var config = await GetConfigurationAsync(tenantId);
            return _encryptionService.Decrypt(config.ConsumerKey);
        }

        public async Task<string> GetDecryptedConsumerSecretAsync(Guid tenantId)
        {
            var config = await GetConfigurationAsync(tenantId);
            return _encryptionService.Decrypt(config.ConsumerSecret);
        }
    }
}
