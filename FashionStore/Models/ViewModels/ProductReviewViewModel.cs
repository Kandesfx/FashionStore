using System;
using System.ComponentModel.DataAnnotations;

namespace FashionStore.Models.ViewModels
{
    public class ProductReviewViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn điểm đánh giá")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5")]
        [Display(Name = "Điểm đánh giá")]
        public int Rating { get; set; }
        
        [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }
        
        [StringLength(2000, ErrorMessage = "Nội dung đánh giá không được quá 2000 ký tự")]
        [Display(Name = "Nội dung đánh giá")]
        public string ReviewText { get; set; }
        
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public int HelpfulCount { get; set; }
        public int NotHelpfulCount { get; set; }
    }
    
    public class ReviewCommentViewModel
    {
        [Required]
        public int ProductReviewId { get; set; }
        
        [Required(ErrorMessage = "Nội dung bình luận không được để trống")]
        [StringLength(1000, ErrorMessage = "Nội dung bình luận không được quá 1000 ký tự")]
        [Display(Name = "Bình luận")]
        public string CommentText { get; set; }
        
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsAdminReply { get; set; }
    }
}

