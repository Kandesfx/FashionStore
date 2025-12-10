using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Địa chỉ khách hàng
    /// </summary>
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Họ và tên người nhận")]
        public string RecipientName { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string StreetAddress { get; set; }

        [StringLength(100)]
        [Display(Name = "Phường/Xã")]
        public string Ward { get; set; }

        [StringLength(100)]
        [Display(Name = "Quận/Huyện")]
        public string District { get; set; }

        [StringLength(100)]
        [Display(Name = "Tỉnh/Thành phố")]
        public string Province { get; set; }

        [StringLength(20)]
        [Display(Name = "Mã bưu điện")]
        public string PostalCode { get; set; }

        [Display(Name = "Địa chỉ mặc định")]
        public bool IsDefault { get; set; } = false;

        [StringLength(50)]
        [Display(Name = "Loại địa chỉ")]
        public string AddressType { get; set; } // Home, Office, Other

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedDate { get; set; }
    }
}

