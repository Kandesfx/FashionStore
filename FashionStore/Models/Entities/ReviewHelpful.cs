using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Đánh dấu review hữu ích/không hữu ích
    /// </summary>
    public class ReviewHelpful
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
        [Display(Name = "Hữu ích")]
        public bool IsHelpful { get; set; } = true; // true = helpful, false = not helpful

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Unique constraint: Mỗi user chỉ vote một lần cho mỗi review
    }
}

