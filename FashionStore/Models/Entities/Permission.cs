using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStore.Models.Entities
{
    /// <summary>
    /// Quyền truy cập
    /// </summary>
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Index("IX_Permissions_Name", IsUnique = true)]
        [Display(Name = "Tên quyền")]
        public string Name { get; set; }

        [StringLength(200)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Module")]
        public string Module { get; set; } // Product, Order, User, Report, etc.

        [Required]
        [StringLength(50)]
        [Display(Name = "Hành động")]
        public string Action { get; set; } // View, Create, Edit, Delete, Export, Approve, Refund

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }

    /// <summary>
    /// Quan hệ nhiều-nhiều giữa Role và Permission
    /// </summary>
    public class RolePermission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }

        [Required]
        [ForeignKey("Permission")]
        public int PermissionId { get; set; }

        public virtual Permission Permission { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

