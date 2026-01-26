using FanQuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<Payment> InitiateEntryFeeAsync(Guid userId, Guid questId, decimal entryFee, string phoneNumber);
        Task<bool> VerifyPaymentAsync(Guid paymentId);
        Task<Payment> InitiateRewardPayoutAsync(Guid userId, Guid questId, decimal amount, string phoneNumber);
        Task InitiateRewardPayoutAsync(Guid userId, decimal value, string phoneNumber);
    }
}
