using FanQuest.Infrastructure.Payments.Models;

namespace FanQuest.Infrastructure.Payments.Mpesa
{
    public interface IMpesaClient
    {
        Task<string> GetAccessTokenAsync(Guid tenantId);
        Task<StkPushResponse> InitiateStkPushAsync(StkPushRequest request, Guid tenantId);
        Task<B2CResponse> InitiateB2CAsync(B2CRequest request, Guid tenantId);
        Task<TransactionStatusResponse> QueryTransactionStatusAsync(string transactionId, Guid tenantId);
    }
}
