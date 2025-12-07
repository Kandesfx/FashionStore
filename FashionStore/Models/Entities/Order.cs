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

        // Navigation properties
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

