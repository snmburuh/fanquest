namespace FanQuest.Domain.Entities
{
        public class MpesaConfiguration
        {
            public Guid Id { get; private set; }
            public Guid TenantId { get; private set; }
            public string ConsumerKey { get; private set; }
            public string ConsumerSecret { get; private set; }
            public string BusinessShortCode { get; private set; }
            public string Passkey { get; private set; }
            public string CallbackUrl { get; private set; }
            public MpesaEnvironment Environment { get; private set; }
        public string StkCallbackUrl { get; set; }
        public string InitiatorName { get; set; }
        public string SecurityCredential { get; set; }
        public string B2CQueueTimeoutUrl { get; set; }
        public string B2CResultUrl { get; set; }
        public double CallbackTimeoutMinutes { get; set; }
        public string BaseUrl { get; set; }

        protected MpesaConfiguration() { }

            public MpesaConfiguration(
                Guid tenantId,
                string consumerKey,
                string consumerSecret,
                string businessShortCode,
                string passkey,
                string callbackUrl,
                MpesaEnvironment environment)
            {
                Id = Guid.NewGuid();
                TenantId = tenantId;
                ConsumerKey = consumerKey;
                ConsumerSecret = consumerSecret;
                BusinessShortCode = businessShortCode;
                Passkey = passkey;
                CallbackUrl = callbackUrl;
                Environment = environment;
            }

            public void UpdateCredentials(string consumerKey, string consumerSecret)
            {
                ConsumerKey = consumerKey;
                ConsumerSecret = consumerSecret;
            }
        }

        public enum MpesaEnvironment
        {
            Sandbox,
            Production
        }
}
