using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Infrastructure.Payments.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FanQuest.Infrastructure.Payments.Mpesa
{
    public class MpesaConfigService : IMpesaConfigService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IConfiguration _configuration;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<MpesaConfigService> _logger;

        public MpesaConfigService(
            ITenantRepository tenantRepository,
            IConfiguration configuration,
            IEncryptionService encryptionService,
            ILogger<MpesaConfigService> logger)
        {
            _tenantRepository = tenantRepository;
            _configuration = configuration;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        public async Task<MpesaConfig> GetConfigForTenantAsync(Guid tenantId)
        {
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);

            if (tenant == null || !tenant.IsActive)
            {
                _logger.LogWarning("Tenant {TenantId} not found or inactive, using default config", tenantId);
                return await GetDefaultConfigAsync();
            }

            if (string.IsNullOrEmpty(tenant.MpesaConfigJson))
            {
                _logger.LogInformation("Tenant {TenantId} has no custom M-Pesa config, using default", tenantId);
                return await GetDefaultConfigAsync();
            }

            try
            {
                var decrypted = _encryptionService.Decrypt(tenant.MpesaConfigJson);
                return JsonSerializer.Deserialize<MpesaConfig>(decrypted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decrypt M-Pesa config for tenant {TenantId}, using default", tenantId);
                return await GetDefaultConfigAsync();
            }
        }

        public Task<MpesaConfig> GetDefaultConfigAsync()
        {
            var config = new MpesaConfig
            {
                ConsumerKey = _configuration["Mpesa:ConsumerKey"],
                ConsumerSecret = _configuration["Mpesa:ConsumerSecret"],
                BusinessShortCode = _configuration["Mpesa:BusinessShortCode"],
                Passkey = _configuration["Mpesa:Passkey"],
                InitiatorName = _configuration["Mpesa:InitiatorName"],
                SecurityCredential = _configuration["Mpesa:SecurityCredential"],
                BaseUrl = _configuration["Mpesa:BaseUrl"],
                StkCallbackUrl = _configuration["Mpesa:StkCallbackUrl"],
                B2CResultUrl = _configuration["Mpesa:B2CResultUrl"],
                B2CQueueTimeoutUrl = _configuration["Mpesa:B2CQueueTimeoutUrl"],
                CallbackTimeoutMinutes = int.Parse(_configuration["Mpesa:CallbackTimeoutMinutes"] ?? "5")
            };

            return Task.FromResult(config);
        }

        Task<MpesaConfig> IMpesaConfigService.GetConfigForTenantAsync(Guid tenantId)
        {
            throw new NotImplementedException();
        }

        //Task<MpesaConfig> IMpesaConfigService.GetConfigForTenantAsync(Guid tenantId)
        //{
        //    throw new NotImplementedException();
        //}

        Task<MpesaConfig> IMpesaConfigService.GetDefaultConfigAsync()
        {
            throw new NotImplementedException();
        }

        //Task<MpesaConfig> IMpesaConfigService.GetDefaultConfigAsync()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
