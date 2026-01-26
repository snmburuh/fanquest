namespace FanQuest.API.Contract.Mpesa
{
    public class B2cCallbackPayload
    {
        public B2cResult Result { get; set; }
    }

    public class B2cResult
    {
        public int ResultType { get; set; }
        public int ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public string OriginatorConversationID { get; set; }
        public string ConversationID { get; set; }
        public string TransactionID { get; set; }
        public B2cResultParameters ResultParameters { get; set; }
    }

    public class B2cResultParameters
    {
        public List<B2cResultParameter> ResultParameter { get; set; }
    }

    public class B2cResultParameter
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}
