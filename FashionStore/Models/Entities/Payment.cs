using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Thanh toán đơn hàng
    /// </summary>
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } // COD, MoMo, VNPay, BankTransfer

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending"; // Pending, Paid, Failed, Refunded

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Số tiền")]
        public decimal Amount { get; set; }

        [StringLength(100)]
        [Display(Name = "Mã giao dịch")]
        public string TransactionId { get; set; }

        [StringLength(100)]
        [Display(Name = "Mã tham chiếu")]
        public string ReferenceCode { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày thanh toán")]
        public DateTime? PaidDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedDate { get; set; }
    }
}

