using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Ảnh đính kèm trong đánh giá
    /// </summary>
    public class ReviewImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("ProductReview")]
        public int ProductReviewId { get; set; }

        public virtual ProductReview ProductReview { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "URL ảnh")]
        public string ImageUrl { get; set; }

        [StringLength(200)]
        [Display(Name = "Tên file")]
        public string FileName { get; set; }

        [Display(Name = "Thứ tự")]
        public int DisplayOrder { get; set; } = 0;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

