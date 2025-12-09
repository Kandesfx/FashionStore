using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Hoàn tiền
    /// </summary>
    public class Refund
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        [ForeignKey("Payment")]
        public int? PaymentId { get; set; }

        public virtual Payment Payment { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Số tiền hoàn")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Loại hoàn")]
        public string RefundType { get; set; } // Full, Partial

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Completed, Rejected

        [StringLength(500)]
        [Display(Name = "Lý do")]
        public string Reason { get; set; }

        [StringLength(100)]
        [Display(Name = "Mã giao dịch hoàn")]
        public string RefundTransactionId { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }

        [ForeignKey("ProcessedBy")]
        public int? ProcessedByUserId { get; set; }

        public virtual User ProcessedBy { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày hoàn thành")]
        public DateTime? CompletedDate { get; set; }
    }
}

