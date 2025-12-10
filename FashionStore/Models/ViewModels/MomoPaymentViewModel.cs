namespace FashionStore.Models.ViewModels
{
    public class MomoPaymentViewModel
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PayUrl { get; set; }
        public string QrCodeUrl { get; set; }
        public string OrderInfo { get; set; }
    }
}

