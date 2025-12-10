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

        [StringLength(255)]
        [Display(Name = "Hình ảnh chi tiết 1")]
        public string Detail1 { get; set; }

        [StringLength(255)]
        [Display(Name = "Hình ảnh chi tiết 2")]
        public string Detail2 { get; set; }

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

        // SEO fields
        [StringLength(200)]
        [Display(Name = "Slug")]
        public string Slug { get; set; }

        [StringLength(200)]
        [Display(Name = "Meta Title")]
        public string MetaTitle { get; set; }

        [StringLength(500)]
        [Display(Name = "Meta Description")]
        public string MetaDescription { get; set; }

        [StringLength(50)]
        [Display(Name = "SKU")]
        public string SKU { get; set; }

        [StringLength(50)]
        [Display(Name = "Barcode")]
        public string Barcode { get; set; }

        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá vốn")]
        public decimal? CostPrice { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Cảnh báo tồn thấp")]
        public int LowStockThreshold { get; set; } = 10;

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
        public virtual ICollection<Media> Media { get; set; }
        public virtual ICollection<ProductCollection> ProductCollections { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }

        // Computed properties
        [NotMapped]
        public decimal FinalPrice => DiscountPrice ?? Price;

        [NotMapped]
        [Display(Name = "Điểm đánh giá trung bình")]
        public decimal? AverageRating { get; set; }

        [NotMapped]
        [Display(Name = "Tổng số đánh giá")]
        public int TotalReviews { get; set; }
    }
}

