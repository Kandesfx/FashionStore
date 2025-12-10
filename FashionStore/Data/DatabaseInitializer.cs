using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FashionStore.Models.Entities;
using BCrypt.Net;

namespace FashionStore.Data
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
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

            // Seed Product Variants cho các sản phẩm
            var productVariants = new List<ProductVariant>();
            
            // Áo Sơ Mi Nam Trắng (Product ID 1)
            var sizes1 = new[] { "S", "M", "L", "XL" };
            var colors1 = new[] { "Trắng", "Xanh", "Đen" };
            foreach (var size in sizes1)
            {
                foreach (var color in colors1)
                {
                    productVariants.Add(new ProductVariant
                    {
                        ProductId = 1,
                        Size = size,
                        Color = color,
                        SKU = $"AO-NAM-001-{size}-{color.Substring(0, 2).ToUpper()}",
                        Price = color == "Trắng" ? (decimal?)249000 : (color == "Xanh" ? (decimal?)259000 : (decimal?)269000),
                        Stock = size == "M" ? 20 : (size == "L" ? 15 : 10),
                        ImageUrl = color == "Trắng" ? "/Image/ao10.jpg" : (color == "Xanh" ? "/Image/ao10_blue.jpg" : "/Image/ao10_black.jpg"),
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    });
                }
            }

            // Áo Thun Nữ Cổ Tròn (Product ID 2)
            var sizes2 = new[] { "XS", "S", "M", "L" };
            var colors2 = new[] { "Hồng", "Trắng", "Đen", "Xanh" };
            foreach (var size in sizes2)
            {
                foreach (var color in colors2)
                {
                    productVariants.Add(new ProductVariant
                    {
                        ProductId = 2,
                        Size = size,
                        Color = color,
                        SKU = $"AO-NU-001-{size}-{color.Substring(0, 2).ToUpper()}",
                        Price = null, // Dùng giá sản phẩm chính
                        Stock = size == "M" ? 25 : (size == "L" ? 20 : 15),
                        ImageUrl = $"/Image/ao11_{color.ToLower()}.jpg",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    });
                }
            }

            // Quần Jean Nam (Product ID 3)
            var sizes3 = new[] { "28", "30", "32", "34", "36" };
            var colors3 = new[] { "Xanh", "Đen" };
            foreach (var size in sizes3)
            {
                foreach (var color in colors3)
                {
                    productVariants.Add(new ProductVariant
                    {
                        ProductId = 3,
                        Size = size,
                        Color = color,
                        SKU = $"QUAN-NAM-001-{size}-{color.Substring(0, 2).ToUpper()}",
                        Price = null,
                        Stock = (size == "30" || size == "32") ? 8 : 5,
                        ImageUrl = color == "Xanh" ? "/Image/ao12.jpg" : "/Image/ao12_black.jpg",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    });
                }
            }

            // Quần Jean Nữ (Product ID 4)
            var sizes4 = new[] { "26", "28", "30", "32" };
            var colors4 = new[] { "Xanh", "Đen", "Trắng" };
            foreach (var size in sizes4)
            {
                foreach (var color in colors4)
                {
                    productVariants.Add(new ProductVariant
                    {
                        ProductId = 4,
                        Size = size,
                        Color = color,
                        SKU = $"QUAN-NU-001-{size}-{color.Substring(0, 2).ToUpper()}",
                        Price = null,
                        Stock = (size == "28" || size == "30") ? 10 : 7,
                        ImageUrl = color == "Xanh" ? "/Image/ao13.jpg" : (color == "Đen" ? "/Image/ao13_black.jpg" : "/Image/ao13_white.jpg"),
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    });
                }
            }

            // Váy Đầm Dạo Phố (Product ID 5)
            var sizes5 = new[] { "S", "M", "L", "XL" };
            var colors5 = new[] { "Hồng", "Xanh", "Trắng", "Đen" };
            foreach (var size in sizes5)
            {
                foreach (var color in colors5)
                {
                    productVariants.Add(new ProductVariant
                    {
                        ProductId = 5,
                        Size = size,
                        Color = color,
                        SKU = $"VAY-001-{size}-{color.Substring(0, 2).ToUpper()}",
                        Price = null,
                        Stock = size == "M" ? 8 : (size == "L" ? 6 : 5),
                        ImageUrl = $"/Image/ao14_{color.ToLower()}.jpg",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    });
                }
            }

            context.ProductVariants.AddRange(productVariants);
            context.SaveChanges();

            // Đồng bộ tồn kho sản phẩm = tổng tồn kho các biến thể (nếu có biến thể)
            foreach (var p in context.Products.ToList())
            {
                var variantsOfProduct = context.ProductVariants.Where(v => v.ProductId == p.Id).ToList();
                if (variantsOfProduct.Any())
                {
                    p.Stock = variantsOfProduct.Sum(v => v.Stock);
                    p.UpdatedDate = DateTime.Now;
                    context.Products.Attach(p);
                    context.Entry(p).State = System.Data.Entity.EntityState.Modified;
                }
            }
            context.SaveChanges();

            // Seed giao dịch tồn kho mẫu để test báo cáo/ls giao dịch
            var firstVariantProd1 = context.ProductVariants.FirstOrDefault(v => v.ProductId == 1);
            var firstVariantProd2 = context.ProductVariants.FirstOrDefault(v => v.ProductId == 2);
            var transactions = new List<InventoryTransaction>();
            if (firstVariantProd1 != null)
            {
                transactions.Add(new InventoryTransaction
                {
                    ProductId = firstVariantProd1.ProductId,
                    ProductVariantId = firstVariantProd1.Id,
                    TransactionType = "Import",
                    Quantity = 30,
                    Notes = "Nhập kho đầu kỳ - áo sơ mi",
                    CreatedByUserId = adminUser.Id,
                    CreatedDate = DateTime.Now.AddDays(-10)
                });
                transactions.Add(new InventoryTransaction
                {
                    ProductId = firstVariantProd1.ProductId,
                    ProductVariantId = firstVariantProd1.Id,
                    TransactionType = "Sale",
                    Quantity = 5,
                    Notes = "Bán hàng",
                    CreatedDate = DateTime.Now.AddDays(-3)
                });
            }
            if (firstVariantProd2 != null)
            {
                transactions.Add(new InventoryTransaction
                {
                    ProductId = firstVariantProd2.ProductId,
                    ProductVariantId = firstVariantProd2.Id,
                    TransactionType = "Import",
                    Quantity = 20,
                    Notes = "Nhập kho đầu kỳ - áo thun",
                    CreatedByUserId = adminUser.Id,
                    CreatedDate = DateTime.Now.AddDays(-8)
                });
                transactions.Add(new InventoryTransaction
                {
                    ProductId = firstVariantProd2.ProductId,
                    ProductVariantId = firstVariantProd2.Id,
                    TransactionType = "Adjustment_Increase",
                    Quantity = 5,
                    Notes = "Kiểm kê tăng thêm",
                    CreatedByUserId = adminUser.Id,
                    CreatedDate = DateTime.Now.AddDays(-2)
                });
            }
            if (transactions.Any())
            {
                context.InventoryTransactions.AddRange(transactions);
                context.SaveChanges();
            }

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
                // Thêm user test
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

                // Seed mã giảm giá
                var coupons = new[]
                {
                    new Coupon
                    {
                        Code = "WELCOME10",
                        Name = "Giảm 10% đơn đầu",
                        Description = "Giảm 10% cho đơn hàng đầu tiên, tối đa 50k",
                        DiscountType = "Percentage",
                        DiscountPercentage = 10,
                        MaximumDiscountAmount = 50000,
                        MinimumOrderAmount = 200000,
                        StartDate = DateTime.Now.AddDays(-10),
                        EndDate = DateTime.Now.AddDays(30),
                        UsageLimit = 100,
                        UsageLimitPerUser = 1,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Coupon
                    {
                        Code = "FIX50K",
                        Name = "Giảm 50k",
                        Description = "Giảm trực tiếp 50k cho đơn từ 400k",
                        DiscountType = "FixedAmount",
                        DiscountAmount = 50000,
                        MinimumOrderAmount = 400000,
                        StartDate = DateTime.Now.AddDays(-5),
                        EndDate = DateTime.Now.AddDays(20),
                        UsageLimit = 200,
                        UsageLimitPerUser = 2,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    }
                };
                context.Coupons.AddRange(coupons);
                context.SaveChanges();

                // Seed khuyến mãi
                var promotions = new[]
                {
                    new Promotion
                    {
                        Name = "Summer Sale 15%",
                        Description = "Giảm 15% cho đơn từ 500k",
                        PromotionType = "Percentage",
                        DiscountPercentage = 15,
                        MinimumOrderAmount = 500000,
                        MaximumDiscountAmount = 80000,
                        StartDate = DateTime.Now.AddDays(-7),
                        EndDate = DateTime.Now.AddDays(14),
                        UsageLimit = 300,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Promotion
                    {
                        Name = "Freeship 30k",
                        Description = "Miễn phí vận chuyển tối đa 30k cho đơn từ 300k",
                        PromotionType = "FreeShipping",
                        DiscountAmount = 30000,
                        MinimumOrderAmount = 300000,
                        StartDate = DateTime.Now.AddDays(-3),
                        EndDate = DateTime.Now.AddDays(10),
                        UsageLimit = 500,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    }
                };
                context.Promotions.AddRange(promotions);
                context.SaveChanges();

                // Seed đơn hàng mẫu + chi tiết
                var orders = new List<Order>
                {
                    new Order
                    {
                        UserId = testUser.Id,
                        OrderDate = DateTime.Now.AddDays(-2),
                        TotalAmount = 750000,
                        Status = "Completed",
                        ShippingAddress = "123 Đường A, Quận 1, TP.HCM",
                        PaymentMethod = "COD",
                        Notes = "Giao giờ hành chính",
                        RecipientPhone = "0987654321",
                        RecipientName = "Test User",
                        ShippingFee = 30000,
                        DiscountAmount = 50000,
                        CouponCode = "FIX50K",
                        CompletedDate = DateTime.Now.AddDays(-1)
                    },
                    new Order
                    {
                        UserId = adminUser.Id,
                        OrderDate = DateTime.Now.AddDays(-1),
                        TotalAmount = 520000,
                        Status = "Pending",
                        ShippingAddress = "456 Đường B, Quận 3, TP.HCM",
                        PaymentMethod = "Momo",
                        Notes = "Liên hệ trước khi giao",
                        RecipientPhone = "0123456789",
                        RecipientName = "Administrator",
                        ShippingFee = 20000,
                        DiscountAmount = 30000,
                        CouponCode = "WELCOME10"
                    }
                };
                context.Orders.AddRange(orders);
                context.SaveChanges();

                // Chi tiết đơn hàng
                var orderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        OrderId = orders[0].Id,
                        ProductId = 1,
                        Quantity = 2,
                        UnitPrice = 249000,
                        SubTotal = 498000
                    },
                    new OrderDetail
                    {
                        OrderId = orders[0].Id,
                        ProductId = 2,
                        Quantity = 1,
                        UnitPrice = 199000,
                        SubTotal = 199000
                    },
                    new OrderDetail
                    {
                        OrderId = orders[1].Id,
                        ProductId = 3,
                        Quantity = 1,
                        UnitPrice = 599000,
                        SubTotal = 599000
                    }
                };
                context.OrderDetails.AddRange(orderDetails);
                context.SaveChanges();

                // Lịch sử dùng coupon
                var couponUsage = new List<CouponUsage>
                {
                    new CouponUsage
                    {
                        CouponId = coupons.First(c => c.Code == "FIX50K").Id,
                        OrderId = orders[0].Id,
                        UserId = testUser.Id,
                        DiscountAmount = 50000,
                        UsedDate = DateTime.Now.AddDays(-2)
                    },
                    new CouponUsage
                    {
                        CouponId = coupons.First(c => c.Code == "WELCOME10").Id,
                        OrderId = orders[1].Id,
                        UserId = adminUser.Id,
                        DiscountAmount = 30000,
                        UsedDate = DateTime.Now.AddDays(-1)
                    }
                };
                context.CouponUsages.AddRange(couponUsage);
                context.SaveChanges();
            }

            base.Seed(context);
        }
    }
}

