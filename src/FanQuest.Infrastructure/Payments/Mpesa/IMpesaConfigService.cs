using FanQuest.Domain.Entities;
using FanQuest.Infrastructure.Payments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Infrastructure.Payments.Mpesa
{
    public interface IMpesaConfigService
    {
        Task<MpesaConfiguration> GetConfigurationAsync(Guid tenantId);
        Task<string> GetDecryptedConsumerKeyAsync(Guid tenantId);
        Task<string> GetDecryptedConsumerSecretAsync(Guid tenantId);
    }
}
