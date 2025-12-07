using System.ComponentModel.DataAnnotations;

namespace FashionStore.Models.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }
        
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }
        
        [Display(Name = "Giá khuyến mãi")]
        public decimal? DiscountPrice { get; set; }
        
        [Required]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }
        
        [Display(Name = "Hình ảnh")]
        public string ImageUrl { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Tồn kho")]
        public int Stock { get; set; }
        
        [Display(Name = "Sản phẩm nổi bật")]
        public bool Featured { get; set; }
        
        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; }
        
        // Additional properties for display
        public string CategoryName { get; set; }
        public decimal FinalPrice => DiscountPrice ?? Price;
    }
}

