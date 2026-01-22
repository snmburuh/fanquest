namespace FanQuest.API.Contract
{
    public record JoinQuestRequest(string PhoneNumber);


    public record CompleteChallengeRequest(DateTime? Timestamp, double? Latitude, double? Longitude, string? Payload);


    public record ClaimRewardRequest(Guid QuestId, string PhoneNumber);
    public record MpesaCallbackDto(string MerchantRequestID, string CheckoutRequestID, int ResultCode, string ResultDesc);
    public record Login_Request(string PhoneNumber, string? DisplayName);
}
