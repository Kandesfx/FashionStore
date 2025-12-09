using Newtonsoft.Json;

namespace FashionStore.Models.Momo
{
    public class MomoCreatePaymentResponse
    {
        [JsonProperty("payUrl")]
        public string PayUrl { get; set; }

        [JsonProperty("qrCodeUrl")]
        public string QrCodeUrl { get; set; }

        [JsonProperty("deeplink")]
        public string Deeplink { get; set; }

        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }
    }
}

