using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Đánh giá sản phẩm của khách hàng
    /// </summary>
    public class ProductReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [ForeignKey("Order")]
        public int? OrderId { get; set; }

        public virtual Order Order { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Điểm đánh giá")]
        public int Rating { get; set; }

        [StringLength(200)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [StringLength(2000)]
        [Display(Name = "Nội dung đánh giá")]
        public string ReviewText { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Reported

        [Display(Name = "Đã mua hàng")]
        public bool IsVerifiedPurchase { get; set; } = false;

        [Display(Name = "Số lượt hữu ích")]
        public int HelpfulCount { get; set; } = 0;

        [Display(Name = "Số lượt không hữu ích")]
        public int NotHelpfulCount { get; set; } = 0;

        [Display(Name = "Số lượt báo cáo")]
        public int ReportCount { get; set; } = 0;

        [StringLength(500)]
        [Display(Name = "Ghi chú admin")]
        public string AdminNotes { get; set; }

        [ForeignKey("ReviewedBy")]
        public int? ReviewedByUserId { get; set; }

        public virtual User ReviewedBy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày duyệt")]
        public DateTime? ReviewedDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public virtual ICollection<ReviewImage> ReviewImages { get; set; }
        public virtual ICollection<ReviewComment> ReviewComments { get; set; }
        public virtual ICollection<ReviewHelpful> ReviewHelpfuls { get; set; }
        public virtual ICollection<ReviewReport> ReviewReports { get; set; }
    }
}

