using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Biến thể sản phẩm (size, màu, v.v.)
    /// </summary>
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        [StringLength(50)]
        [Display(Name = "SKU")]
        public string SKU { get; set; }

        [StringLength(50)]
        [Display(Name = "Barcode")]
        public string Barcode { get; set; }

        [StringLength(50)]
        [Display(Name = "Size")]
        public string Size { get; set; }

        [StringLength(50)]
        [Display(Name = "Màu")]
        public string Color { get; set; }

        [StringLength(100)]
        [Display(Name = "Tên biến thể")]
        public string VariantName { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Giá")]
        public decimal? Price { get; set; } // Giá riêng cho biến thể, null thì dùng giá Product

        [Range(0, int.MaxValue)]
        [Display(Name = "Tồn kho")]
        public int Stock { get; set; } = 0;

        [StringLength(255)]
        [Display(Name = "Ảnh")]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedDate { get; set; }
    }
}

