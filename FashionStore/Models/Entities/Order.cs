using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        [Range(0, double.MaxValue)]
        [DataType(DataType.Currency)]
        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending";

        [Required]
        [StringLength(500)]
        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }

        [StringLength(50)]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }

        [StringLength(20)]
        [Display(Name = "Số điện thoại người nhận")]
        public string RecipientPhone { get; set; }

        [StringLength(100)]
        [Display(Name = "Tên người nhận")]
        public string RecipientName { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày duyệt")]
        public DateTime? ApprovedDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày đóng gói")]
        public DateTime? PackedDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày giao hàng")]
        public DateTime? ShippedDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày hoàn thành")]
        public DateTime? CompletedDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày hủy")]
        public DateTime? CancelledDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Lý do hủy")]
        public string CancellationReason { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Phí vận chuyển")]
        public decimal ShippingFee { get; set; } = 0;

        [Range(0, double.MaxValue)]
        [Display(Name = "Giảm giá")]
        public decimal DiscountAmount { get; set; } = 0;

        [StringLength(50)]
        [Display(Name = "Mã giảm giá")]
        public string CouponCode { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Refund> Refunds { get; set; }
        public virtual ICollection<Shipment> Shipments { get; set; }
        public virtual ICollection<Return> Returns { get; set; }
        public virtual ICollection<CouponUsage> CouponUsages { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
    }
}

