using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Khuyến mãi
    /// </summary>
    public class Promotion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Tên khuyến mãi")]
        public string Name { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Loại")]
        public string PromotionType { get; set; } // Percentage, FixedAmount, BuyXGetY, FreeShipping

        [Range(0, 100)]
        [Display(Name = "Phần trăm giảm")]
        public decimal? DiscountPercentage { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Số tiền giảm")]
        public decimal? DiscountAmount { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Giá trị đơn tối thiểu")]
        public decimal? MinimumOrderAmount { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Giá trị đơn tối đa")]
        public decimal? MaximumDiscountAmount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Giới hạn số lượt")]
        public int? UsageLimit { get; set; }

        [Display(Name = "Số lượt đã dùng")]
        public int UsedCount { get; set; } = 0;

        [Required]
        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedDate { get; set; }
    }
}

