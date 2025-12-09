using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Vận chuyển đơn hàng
    /// </summary>
    public class Shipment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        [StringLength(100)]
        [Display(Name = "Hãng vận chuyển")]
        public string ShippingCompany { get; set; } // GHTK, GHN, ViettelPost, etc.

        [StringLength(100)]
        [Display(Name = "Mã vận đơn")]
        public string TrackingNumber { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending"; // Pending, Packed, Shipped, InTransit, Delivered, Failed

        [Range(0, double.MaxValue)]
        [Display(Name = "Phí vận chuyển")]
        public decimal ShippingFee { get; set; }

        [StringLength(500)]
        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }

        [StringLength(20)]
        [Display(Name = "Số điện thoại người nhận")]
        public string RecipientPhone { get; set; }

        [StringLength(100)]
        [Display(Name = "Tên người nhận")]
        public string RecipientName { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày đóng gói")]
        public DateTime? PackedDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày giao hàng")]
        public DateTime? ShippedDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày giao thành công")]
        public DateTime? DeliveredDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedDate { get; set; }
    }
}

