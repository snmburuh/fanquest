using System.Text.Json.Serialization;

namespace FanQuest.Infrastructure.Payments.Models
{
    // Token
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }

    // STK Push
    public class StkPushRequest
    {
        [JsonPropertyName("BusinessShortCode")]
        public string BusinessShortCode { get; set; }

        [JsonPropertyName("Password")]
        public string Password { get; set; }

        [JsonPropertyName("Timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("TransactionType")]
        public string TransactionType { get; set; } = "CustomerPayBillOnline";

        [JsonPropertyName("Amount")]
        public string Amount { get; set; }

        [JsonPropertyName("PartyA")]
        public string PartyA { get; set; }

        [JsonPropertyName("PartyB")]
        public string PartyB { get; set; }

        [JsonPropertyName("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("CallBackURL")]
        public string CallBackURL { get; set; }

        [JsonPropertyName("AccountReference")]
        public string AccountReference { get; set; }

        [JsonPropertyName("TransactionDesc")]
        public string TransactionDesc { get; set; }
    }

    public class StkPushResponse
    {
        [JsonPropertyName("MerchantRequestID")]
        public string MerchantRequestID { get; set; }

        [JsonPropertyName("CheckoutRequestID")]
        public string CheckoutRequestID { get; set; }

        [JsonPropertyName("ResponseCode")]
        public string ResponseCode { get; set; }

        [JsonPropertyName("ResponseDescription")]
        public string ResponseDescription { get; set; }

        [JsonPropertyName("CustomerMessage")]
        public string CustomerMessage { get; set; }
    }

    // B2C
    public class B2CRequest
    {
        [JsonPropertyName("OriginatorConversationID")]
        public string OriginatorConversationID { get; set; }

        [JsonPropertyName("InitiatorName")]
        public string InitiatorName { get; set; }

        [JsonPropertyName("SecurityCredential")]
        public string SecurityCredential { get; set; }

        [JsonPropertyName("CommandID")]
        public string CommandID { get; set; } = "BusinessPayment";

        [JsonPropertyName("Amount")]
        public string Amount { get; set; }

        [JsonPropertyName("PartyA")]
        public string PartyA { get; set; }

        [JsonPropertyName("PartyB")]
        public string PartyB { get; set; }

        [JsonPropertyName("Remarks")]
        public string Remarks { get; set; }

        [JsonPropertyName("QueueTimeOutURL")]
        public string QueueTimeOutURL { get; set; }

        [JsonPropertyName("ResultURL")]
        public string ResultURL { get; set; }

        [JsonPropertyName("Occassion")]
        public string Occassion { get; set; }
    }

    public class B2CResponse
    {
        [JsonPropertyName("ConversationID")]
        public string ConversationID { get; set; }

        [JsonPropertyName("OriginatorConversationID")]
        public string OriginatorConversationID { get; set; }

        [JsonPropertyName("ResponseCode")]
        public string ResponseCode { get; set; }

        [JsonPropertyName("ResponseDescription")]
        public string ResponseDescription { get; set; }
    }

    // Transaction Status
    public class TransactionStatusRequest
    {
        [JsonPropertyName("Initiator")]
        public string Initiator { get; set; }

        [JsonPropertyName("SecurityCredential")]
        public string SecurityCredential { get; set; }

        [JsonPropertyName("CommandID")]
        public string CommandID { get; set; } = "TransactionStatusQuery";

        [JsonPropertyName("TransactionID")]
        public string TransactionID { get; set; }

        [JsonPropertyName("OriginalConversationID")]
        public string OriginalConversationID { get; set; }

        [JsonPropertyName("PartyA")]
        public string PartyA { get; set; }

        [JsonPropertyName("IdentifierType")]
        public string IdentifierType { get; set; }

        [JsonPropertyName("ResultURL")]
        public string ResultURL { get; set; }

        [JsonPropertyName("QueueTimeOutURL")]
        public string QueueTimeOutURL { get; set; }

        [JsonPropertyName("Remarks")]
        public string Remarks { get; set; }

        [JsonPropertyName("Occasion")]
        public string Occasion { get; set; }
    }

    public class TransactionStatusResponse
    {
        [JsonPropertyName("ConversationID")]
        public string ConversationID { get; set; }

        [JsonPropertyName("OriginatorConversationID")]
        public string OriginatorConversationID { get; set; }

        [JsonPropertyName("ResponseCode")]
        public string ResponseCode { get; set; }

        [JsonPropertyName("ResponseDescription")]
        public string ResponseDescription { get; set; }
    }
}
