using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Bình luận/phản hồi đánh giá (của khách hàng khác hoặc admin)
    /// </summary>
    public class ReviewComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("ProductReview")]
        public int ProductReviewId { get; set; }

        public virtual ProductReview ProductReview { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        [StringLength(1000)]
        [Display(Name = "Nội dung bình luận")]
        public string CommentText { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Approved"; // Approved, Pending, Rejected, Reported

        [Display(Name = "Là phản hồi của admin")]
        public bool IsAdminReply { get; set; } = false;

        [ForeignKey("ParentComment")]
        public int? ParentCommentId { get; set; }

        public virtual ReviewComment ParentComment { get; set; }

        [Display(Name = "Số lượt báo cáo")]
        public int ReportCount { get; set; } = 0;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public virtual ICollection<ReviewComment> ChildComments { get; set; }
    }
}

