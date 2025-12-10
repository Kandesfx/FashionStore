using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(100)]
        [Index("IX_Categories_CategoryName", IsUnique = true)]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [StringLength(255)]
        [Display(Name = "Hình ảnh")]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; } = 0;

        [Required]
        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        [ForeignKey("ParentCategory")]
        public int? ParentCategoryId { get; set; }

        public virtual Category ParentCategory { get; set; }

        [StringLength(200)]
        [Display(Name = "Slug")]
        public string Slug { get; set; }

        [StringLength(200)]
        [Display(Name = "Meta Title")]
        public string MetaTitle { get; set; }

        [StringLength(500)]
        [Display(Name = "Meta Description")]
        public string MetaDescription { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Category> SubCategories { get; set; }
    }
}

