using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Quan hệ nhiều-nhiều giữa Product và Collection
    /// </summary>
    public class ProductCollection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        [Required]
        [ForeignKey("Collection")]
        public int CollectionId { get; set; }

        public virtual Collection Collection { get; set; }

        [Display(Name = "Thứ tự")]
        public int DisplayOrder { get; set; } = 0;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

