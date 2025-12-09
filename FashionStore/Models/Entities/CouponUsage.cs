using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Lịch sử sử dụng mã giảm giá
    /// </summary>
    public class CouponUsage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Coupon")]
        public int CouponId { get; set; }

        public virtual Coupon Coupon { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Số tiền giảm")]
        public decimal DiscountAmount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày sử dụng")]
        public DateTime UsedDate { get; set; } = DateTime.Now;
    }
}

