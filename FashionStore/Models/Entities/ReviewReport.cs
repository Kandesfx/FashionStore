using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Báo cáo đánh giá/bình luận không phù hợp
    /// </summary>
    public class ReviewReport
    {
        [Key]
        public int Id { get; set; }

        public int? ProductReviewId { get; set; }

        public virtual ProductReview ProductReview { get; set; }

        public int? ReviewCommentId { get; set; }

        public virtual ReviewComment ReviewComment { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Loại báo cáo")]
        public string ReportType { get; set; } // Spam, Inappropriate, Fake, Offensive, Other

        [Required]
        [StringLength(500)]
        [Display(Name = "Lý do")]
        public string Reason { get; set; }

        [StringLength(1000)]
        [Display(Name = "Mô tả chi tiết")]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending"; // Pending, Reviewed, Resolved, Dismissed

        [ForeignKey("ReviewedBy")]
        public int? ReviewedByUserId { get; set; }

        public virtual User ReviewedBy { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú xử lý")]
        public string AdminNotes { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày xử lý")]
        public DateTime? ReviewedDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

