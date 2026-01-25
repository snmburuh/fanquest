namespace FanQuest.API.Contract
{
    public record JoinQuestRequest(string PhoneNumber);


    public record CompleteChallengeRequest(DateTime? Timestamp, double? Latitude, double? Longitude, string? Payload);


    public record ClaimRewardRequest(Guid QuestId, string PhoneNumber);
    public record MpesaCallbackDto(string MerchantRequestID, string CheckoutRequestID, int ResultCode, string ResultDesc);
    public record Login_Request(string PhoneNumber, string? DisplayName);

    public record CreateQuestRequest(
    string Name,
    string City,
    DateTime StartsAt,
    DateTime EndsAt,
    decimal EntryFee,
    bool PublishImmediately = false
);

    public record CreateChallengeRequest(
    string Title,
    int Points,
    DateTime OpensAt,
    DateTime ClosesAt,
    string Type, // "CheckIn", "Timed", or "Reaction"
    string? LocationName = null // Only for CheckIn challenges
);

    public record CreateRewardRequest(
    string Name,
    decimal Value,
    int MinRank,
    int MaxRank
);


}
