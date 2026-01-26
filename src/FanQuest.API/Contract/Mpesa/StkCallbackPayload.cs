namespace FanQuest.API.Contract.Mpesa
{
    public class StkCallbackPayload
    {
        public StkCallbackBody Body { get; set; }
    }

    public class StkCallbackBody
    {
        public StkCallback StkCallback { get; set; }
    }

    public class StkCallback
    {
        public string MerchantRequestID { get; set; }
        public string CheckoutRequestID { get; set; }
        public int ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public CallbackMetadata CallbackMetadata { get; set; }
    }

    public class CallbackMetadata
    {
        public List<CallbackMetadataItem> Item { get; set; }
    }

    public class CallbackMetadataItem
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
