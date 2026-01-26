using FanQuest.Infrastructure.Payments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanQuest.Infrastructure.Payments.Mpesa
{
    public interface IMpesaClient
    {
        Task<string> GetAccessTokenAsync(MpesaConfig config);
        Task<StkPushResponse> InitiateStkPushAsync(MpesaConfig config, StkPushRequest request);
        Task<B2CResponse> InitiateB2CAsync(MpesaConfig config, B2CRequest request);
        Task<TransactionStatusResponse> QueryTransactionStatusAsync(MpesaConfig config, TransactionStatusRequest request);
    }
}
