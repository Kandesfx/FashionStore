using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace FashionStore.Models.ViewModels
{
    public class CreateReviewViewModel
    {
        [Required(ErrorMessage = "Sản phẩm là bắt buộc")]
        public int ProductId { get; set; }

        public int? OrderId { get; set; }

        [Required(ErrorMessage = "Điểm đánh giá là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5")]
        [Display(Name = "Điểm đánh giá")]
        public int Rating { get; set; }

        [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [StringLength(2000, ErrorMessage = "Nội dung đánh giá không được quá 2000 ký tự")]
        [Display(Name = "Nội dung đánh giá")]
        public string ReviewText { get; set; }

        [Display(Name = "Ảnh đính kèm")]
        public List<HttpPostedFileBase> Images { get; set; }

        public List<string> ImageUrls { get; set; } // For existing images

        public bool IsVerifiedPurchase { get; set; }
    }
}

