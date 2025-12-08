using System.ComponentModel.DataAnnotations;

namespace FashionStore.Models.ViewModels
{
    public class VerifyCodeViewModel
    {
        [Required(ErrorMessage = "Mã xác nhận là bắt buộc")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã xác nhận phải có 6 số")]
        [Display(Name = "Mã xác nhận")]
        public string Code { get; set; }

        public string Email { get; set; }
    }
}

