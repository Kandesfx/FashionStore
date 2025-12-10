using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3-50 ký tự")]
        [Index("IX_Users_Username", IsUnique = true)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        [Index("IX_Users_Email", IsUnique = true)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [StringLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public bool IsActive { get; set; } = true;

        [StringLength(50)]
        [Display(Name = "Giới tính")]
        public string Gender { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        [Display(Name = "Kênh chuyển đổi")]
        public string ConversionChannel { get; set; } // Facebook, Google, Direct, etc.

        [Display(Name = "Tổng giá trị đơn hàng")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalOrderValue { get; set; } = 0;

        [Display(Name = "Số đơn hàng")]
        public int TotalOrders { get; set; } = 0;

        [StringLength(500)]
        [Display(Name = "Ghi chú nội bộ")]
        public string InternalNotes { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Lần đăng nhập cuối")]
        public DateTime? LastLoginDate { get; set; }

        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Refund> Refunds { get; set; }
        public virtual ICollection<Return> Returns { get; set; }
        public virtual ICollection<CouponUsage> CouponUsages { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        public virtual ICollection<ReviewComment> ReviewComments { get; set; }
        public virtual ICollection<ReviewHelpful> ReviewHelpfuls { get; set; }
        public virtual ICollection<ReviewReport> ReviewReports { get; set; }
    }
}

