using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<string> InitiateEntryFeeAsync(Guid userId, Guid questId, decimal amount, string phoneNumber);
        Task<bool> VerifyPaymentAsync(string mpesaReceipt);
        Task InitiateRewardPayoutAsync(Guid userId, decimal amount, string phoneNumber);
    }
}
