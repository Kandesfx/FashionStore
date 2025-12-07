using System.ComponentModel.DataAnnotations;

namespace FashionStore.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [StringLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [StringLength(255)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}

