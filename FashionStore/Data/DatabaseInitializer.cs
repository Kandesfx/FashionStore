using System;
using System.Data.Entity;
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
                new Category { CategoryName = "Áo Nam", Description = "Áo sơ mi, áo thun nam", DisplayOrder = 1, IsActive = true, CreatedDate = DateTime.Now },
                new Category { CategoryName = "Áo Nữ", Description = "Áo sơ mi, áo thun nữ", DisplayOrder = 2, IsActive = true, CreatedDate = DateTime.Now },
                new Category { CategoryName = "Quần Nam", Description = "Quần jean, quần tây nam", DisplayOrder = 3, IsActive = true, CreatedDate = DateTime.Now },
                new Category { CategoryName = "Quần Nữ", Description = "Quần jean, quần short nữ", DisplayOrder = 4, IsActive = true, CreatedDate = DateTime.Now },
                new Category { CategoryName = "Váy", Description = "Váy đầm, chân váy", DisplayOrder = 5, IsActive = true, CreatedDate = DateTime.Now }
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
                    ImageUrl = "/images/products/ao-so-mi-nam-trang.jpg",
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
                    ImageUrl = "/images/products/ao-thun-nu.jpg",
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
                    ImageUrl = "/images/products/quan-jean-nam.jpg",
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
                    ImageUrl = "/images/products/quan-jean-nu.jpg",
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
                    ImageUrl = "/images/products/vay-dam.jpg",
                    Stock = 25,
                    Featured = true,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            };
            context.Products.AddRange(products);

            context.SaveChanges();
            base.Seed(context);
        }
    }
}

