using System.ComponentModel.DataAnnotations;

namespace FashionStore.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Họ tên người nhận là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Họ tên người nhận")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
        [StringLength(500)]
        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }

        [StringLength(50)]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }
    }
}

