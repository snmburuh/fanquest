namespace FanQuest.Infrastructure.Payments.Models
{
    public class MpesaConfig
    {
        public string ConsumerKey { get; set; } = string.Empty;
        public string ConsumerSecret { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string PassKey { get; set; } = string.Empty;
        public string? Passkey { get; internal set; }
        public string ApiUrl { get; set; } = "https://api.safaricom.co.ke";
        public string CallbackUrl { get; set; } = string.Empty;
        public string? BusinessShortCode { get; internal set; }
        public string? InitiatorName { get; internal set; }
        public string? B2CQueueTimeoutUrl { get; internal set; }
        public int CallbackTimeoutMinutes { get; internal set; }
        public string? SecurityCredential { get; internal set; }
        public string? BaseUrl { get; internal set; }
        public string? StkCallbackUrl { get; internal set; }
        public string? B2CResultUrl { get; internal set; }
    }

    public class MpesaTenantConfig
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public MpesaConfig MpesaConfig { get; set; }
        public bool IsActive { get; set; }
    }
}
