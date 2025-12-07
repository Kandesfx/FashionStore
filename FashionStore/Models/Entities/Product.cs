using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200)]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Display(Name = "Mô tả")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá khuyến mãi phải lớn hơn 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Giá khuyến mãi")]
        public decimal? DiscountPrice { get; set; }

        [Required]
        [ForeignKey("Category")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        [StringLength(255)]
        [Display(Name = "Hình ảnh")]
        public string ImageUrl { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải >= 0")]
        [Display(Name = "Tồn kho")]
        public int Stock { get; set; } = 0;

        [Required]
        [Display(Name = "Sản phẩm nổi bật")]
        public bool Featured { get; set; } = false;

        [Required]
        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }

        // Computed property
        [NotMapped]
        public decimal FinalPrice => DiscountPrice ?? Price;
    }
}

