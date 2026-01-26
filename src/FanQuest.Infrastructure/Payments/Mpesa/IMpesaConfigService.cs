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
        Task<MpesaConfig> GetConfigForTenantAsync(Guid tenantId);
        Task<MpesaConfig> GetDefaultConfigAsync();
    }
}
