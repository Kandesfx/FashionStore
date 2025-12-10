using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Đổi trả hàng (RMA - Return Merchandise Authorization)
    /// </summary>
    public class Return
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Loại yêu cầu")]
        public string ReturnType { get; set; } // Return (hoàn tiền), Exchange (đổi hàng)

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Requested"; // Requested, Approved, Rejected, Received, Completed

        [Required]
        [StringLength(500)]
        [Display(Name = "Lý do")]
        public string Reason { get; set; }

        [StringLength(1000)]
        [Display(Name = "Mô tả chi tiết")]
        public string Description { get; set; }

        [StringLength(100)]
        [Display(Name = "Mã vận đơn hoàn")]
        public string ReturnTrackingNumber { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }

        [ForeignKey("ProcessedBy")]
        public int? ProcessedByUserId { get; set; }

        public virtual User ProcessedBy { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày yêu cầu")]
        public DateTime RequestedDate { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày duyệt")]
        public DateTime? ApprovedDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày nhận hàng")]
        public DateTime? ReceivedDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày hoàn thành")]
        public DateTime? CompletedDate { get; set; }
    }
}

