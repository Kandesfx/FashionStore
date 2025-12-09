using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Nhật ký audit - ghi nhận các thay đổi quan trọng
    /// </summary>
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Bảng")]
        public string TableName { get; set; }

        [Required]
        [Display(Name = "ID bản ghi")]
        public int RecordId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Hành động")]
        public string Action { get; set; } // Create, Update, Delete

        [StringLength(100)]
        [Display(Name = "Trường")]
        public string FieldName { get; set; }

        [Display(Name = "Giá trị cũ")]
        [Column(TypeName = "ntext")]
        public string OldValue { get; set; }

        [Display(Name = "Giá trị mới")]
        [Column(TypeName = "ntext")]
        public string NewValue { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }

        public virtual User User { get; set; }

        [StringLength(50)]
        [Display(Name = "IP")]
        public string IpAddress { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

