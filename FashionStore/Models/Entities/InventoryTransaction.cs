using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Giao dịch tồn kho (nhập/xuất/điều chỉnh)
    /// </summary>
    public class InventoryTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        [ForeignKey("ProductVariant")]
        public int? ProductVariantId { get; set; }

        public virtual ProductVariant ProductVariant { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Loại giao dịch")]
        public string TransactionType { get; set; } // Import (nhập), Export (xuất), Adjustment (điều chỉnh), Return (hoàn hàng)

        [Required]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; } // Số dương cho Import, số âm cho Export

        [StringLength(500)]
        [Display(Name = "Lý do")]
        public string Reason { get; set; }

        [StringLength(100)]
        [Display(Name = "Mã tham chiếu")]
        public string ReferenceCode { get; set; } // Mã đơn hàng, phiếu nhập, v.v.

        [ForeignKey("Order")]
        public int? OrderId { get; set; }

        public virtual Order Order { get; set; }

        [ForeignKey("CreatedBy")]
        public int? CreatedByUserId { get; set; }

        public virtual User CreatedBy { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }
    }
}

