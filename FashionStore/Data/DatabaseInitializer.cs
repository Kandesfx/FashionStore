using System;
using System.Data.Entity;
using System.Linq;
using FashionStore.Models.Entities;
using BCrypt.Net;

namespace FashionStore.Data
{
    public class DatabaseInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            // Seed Roles
            var adminRole = new Role { RoleName = "Admin", Description = "Quản trị viên" };
            var userRole = new Role { RoleName = "User", Description = "Người dùng" };
            context.Roles.Add(adminRole);
            context.Roles.Add(userRole);

            // Seed Admin User
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@fashionstore.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FullName = "Administrator",
                Phone = "0123456789",
                RoleId = 1, // Admin role
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            context.Users.Add(adminUser);

            // Seed Categories
            var categories = new[]
            {
                new Category { CategoryName = "Áo Nam", Description = "Áo sơ mi, áo thun nam", Slug = "ao-nam", DisplayOrder = 1, IsActive = true, CreatedDate = DateTime.Now },
                new Category { CategoryName = "Áo Nữ", Description = "Áo sơ mi, áo thun nữ", Slug = "ao-nu", DisplayOrder = 2, IsActive = true, CreatedDate = DateTime.Now },
                new Category { CategoryName = "Quần Nam", Description = "Quần jean, quần tây nam", Slug = "quan-nam", DisplayOrder = 3, IsActive = true, CreatedDate = DateTime.Now },
                new Category { CategoryName = "Quần Nữ", Description = "Quần jean, quần short nữ", Slug = "quan-nu", DisplayOrder = 4, IsActive = true, CreatedDate = DateTime.Now },
                new Category { CategoryName = "Váy", Description = "Váy đầm, chân váy", Slug = "vay", DisplayOrder = 5, IsActive = true, CreatedDate = DateTime.Now }
            };
            context.Categories.AddRange(categories);

            // Seed Products
            var products = new[]
            {
                new Product
                {
                    ProductName = "Áo Sơ Mi Nam Trắng",
                    Description = "Áo sơ mi nam form rộng, chất liệu cotton cao cấp, thoáng mát, dễ mặc",
                    Price = 299000,
                    DiscountPrice = 249000,
                    CategoryId = 1,
                    SKU = "AO-NAM-001",
                    ImageUrl = "/Image/ao10.jpg",
                    Detail1 = "/Image/ao10_detail1.jpg",
                    Detail2 = "/Image/ao10_detail2.jpg",
                    Stock = 50,
                    Featured = true,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    ProductName = "Áo Thun Nữ Cổ Tròn",
                    Description = "Áo thun nữ basic, nhiều màu sắc, chất liệu cotton mềm mại",
                    Price = 199000,
                    DiscountPrice = 149000,
                    CategoryId = 2,
                    SKU = "AO-NU-001",
                    ImageUrl = "/Image/ao11.jpg",
                    Detail1 = "/Image/ao11_detail1.jpg",
                    Detail2 = "/Image/ao11_detail2.jpg",
                    Stock = 100,
                    Featured = true,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    ProductName = "Quần Jean Nam",
                    Description = "Quần jean nam form slim, chất liệu denim cao cấp, bền đẹp",
                    Price = 599000,
                    CategoryId = 3,
                    SKU = "QUAN-NAM-001",
                    ImageUrl = "/Image/ao12.jpg",
                    Detail1 = "/Image/ao12_detail1.jpg",
                    Detail2 = "/Image/ao12_detail2.jpg",
                    Stock = 30,
                    Featured = false,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    ProductName = "Quần Jean Nữ",
                    Description = "Quần jean nữ form skinny, tôn dáng, nhiều size",
                    Price = 549000,
                    DiscountPrice = 449000,
                    CategoryId = 4,
                    SKU = "QUAN-NU-001",
                    ImageUrl = "/Image/ao13.jpg",
                    Detail1 = "/Image/ao13_detail1.jpg",
                    Detail2 = "/Image/ao13_detail2.jpg",
                    Stock = 40,
                    Featured = true,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    ProductName = "Váy Đầm Dạo Phố",
                    Description = "Váy đầm nữ dạo phố, thiết kế trẻ trung, năng động",
                    Price = 399000,
                    CategoryId = 5,
                    SKU = "VAY-001",
                    ImageUrl = "/Image/ao14.jpg",
                    Detail1 = "/Image/ao14_detail1.jpg",
                    Detail2 = "/Image/ao14_detail2.jpg",
                    Stock = 25,
                    Featured = true,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            };
            context.Products.AddRange(products);
            context.SaveChanges();

            // Seed Permissions (cơ bản cho admin)
            var permissions = new[]
            {
                new Permission { Name = "Product.View", Description = "Xem sản phẩm", Module = "Product", Action = "View", CreatedDate = DateTime.Now },
                new Permission { Name = "Product.Create", Description = "Tạo sản phẩm", Module = "Product", Action = "Create", CreatedDate = DateTime.Now },
                new Permission { Name = "Product.Edit", Description = "Sửa sản phẩm", Module = "Product", Action = "Edit", CreatedDate = DateTime.Now },
                new Permission { Name = "Product.Delete", Description = "Xóa sản phẩm", Module = "Product", Action = "Delete", CreatedDate = DateTime.Now },
                new Permission { Name = "Order.View", Description = "Xem đơn hàng", Module = "Order", Action = "View", CreatedDate = DateTime.Now },
                new Permission { Name = "Order.Edit", Description = "Sửa đơn hàng", Module = "Order", Action = "Edit", CreatedDate = DateTime.Now },
                new Permission { Name = "Review.Manage", Description = "Quản lý đánh giá", Module = "Review", Action = "Manage", CreatedDate = DateTime.Now },
                new Permission { Name = "Review.Approve", Description = "Duyệt đánh giá", Module = "Review", Action = "Approve", CreatedDate = DateTime.Now },
                new Permission { Name = "Review.Delete", Description = "Xóa đánh giá", Module = "Review", Action = "Delete", CreatedDate = DateTime.Now }
            };
            context.Permissions.AddRange(permissions);
            context.SaveChanges();

            // Gán tất cả quyền cho Admin role
            var adminRoleId = adminRole.Id;
            foreach (var permission in permissions)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRoleId,
                    PermissionId = permission.Id,
                    CreatedDate = DateTime.Now
                });
            }
            context.SaveChanges();

            // Seed mẫu đánh giá sản phẩm (sau khi có products và users)
            if (context.Users.Any() && context.Products.Any())
            {
                var testUser = new User
                {
                    Username = "testuser",
                    Email = "test@fashionstore.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                    FullName = "Test User",
                    Phone = "0987654321",
                    RoleId = userRole.Id,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                context.Users.Add(testUser);
                context.SaveChanges();

                var sampleReviews = new[]
                {
                    new ProductReview
                    {
                        ProductId = 1,
                        UserId = testUser.Id,
                        Rating = 5,
                        Title = "Sản phẩm tuyệt vời!",
                        ReviewText = "Áo sơ mi này rất đẹp, chất liệu tốt, mặc rất thoải mái. Đáng giá tiền!",
                        Status = "Approved",
                        IsVerifiedPurchase = true,
                        CreatedDate = DateTime.Now.AddDays(-5)
                    },
                    new ProductReview
                    {
                        ProductId = 1,
                        UserId = adminUser.Id,
                        Rating = 4,
                        Title = "Khá ổn",
                        ReviewText = "Chất lượng tốt nhưng giá hơi cao một chút.",
                        Status = "Approved",
                        IsVerifiedPurchase = false,
                        CreatedDate = DateTime.Now.AddDays(-3)
                    },
                    new ProductReview
                    {
                        ProductId = 2,
                        UserId = testUser.Id,
                        Rating = 5,
                        Title = "Rất hài lòng",
                        ReviewText = "Áo thun mặc rất mát, form đẹp, nhiều màu để chọn.",
                        Status = "Approved",
                        IsVerifiedPurchase = true,
                        CreatedDate = DateTime.Now.AddDays(-2)
                    }
                };
                context.ProductReviews.AddRange(sampleReviews);
                context.SaveChanges();

                // Seed mẫu bình luận
                if (sampleReviews.Any())
                {
                    var sampleComments = new[]
                    {
                        new ReviewComment
                        {
                            ProductReviewId = sampleReviews[0].Id,
                            UserId = adminUser.Id,
                            CommentText = "Cảm ơn bạn đã đánh giá! Chúng tôi sẽ tiếp tục cải thiện chất lượng sản phẩm.",
                            Status = "Approved",
                            IsAdminReply = true,
                            CreatedDate = DateTime.Now.AddDays(-4)
                        }
                    };
                    context.ReviewComments.AddRange(sampleComments);
                    context.SaveChanges();
                }
            }

            base.Seed(context);
        }
    }
}

