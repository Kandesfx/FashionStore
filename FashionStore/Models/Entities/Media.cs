using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Media (ảnh, video) của sản phẩm
    /// </summary>
    public class Media
    {
        [Key]
        public int Id { get; set; }

        public int? ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int? ProductVariantId { get; set; }

        public virtual ProductVariant ProductVariant { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "URL")]
        public string Url { get; set; }

        [StringLength(200)]
        [Display(Name = "Tên file")]
        public string FileName { get; set; }

        [StringLength(50)]
        [Display(Name = "Loại")]
        public string MediaType { get; set; } // Image, Video

        [StringLength(50)]
        [Display(Name = "Kích thước")]
        public string Size { get; set; } // Thumbnail, Small, Medium, Large

        [Display(Name = "Thứ tự")]
        public int DisplayOrder { get; set; } = 0;

        [StringLength(200)]
        [Display(Name = "Alt text")]
        public string AltText { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

