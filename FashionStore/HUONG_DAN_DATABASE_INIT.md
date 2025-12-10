# HƯỚNG DẪN DATABASE INITIALIZATION

## Tổng quan

Hệ thống sử dụng **Entity Framework Code First** với `CreateDatabaseIfNotExists` strategy. Khi bạn xóa database và chạy lại ứng dụng lần đầu, tất cả các bảng sẽ được **tự động tạo** từ các Entity classes trong `Models/Entities/`.

## Cách hoạt động

### 1. Database Initializer

File `FashionStore/Data/DatabaseInitializer.cs` kế thừa từ `CreateDatabaseIfNotExists<ApplicationDbContext>`:

```csharp
public class DatabaseInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
{
    protected override void Seed(ApplicationDbContext context)
    {
        // Seed dữ liệu mẫu
    }
}
```

### 2. Cấu hình trong ApplicationDbContext

```csharp
public ApplicationDbContext() : base("DefaultConnection")
{
    Database.SetInitializer<ApplicationDbContext>(new DatabaseInitializer());
}
```

### 3. Quá trình tự động

Khi ứng dụng khởi động lần đầu:
1. Entity Framework kiểm tra database có tồn tại không
2. Nếu không tồn tại → Tự động tạo database
3. Tự động tạo **TẤT CẢ** các bảng từ các `DbSet` trong `ApplicationDbContext`
4. Tạo các **Foreign Keys**, **Indexes**, **Constraints** đã được cấu hình
5. Chạy method `Seed()` để thêm dữ liệu mẫu

## Các bảng được tự động tạo

### Bảng cơ bản
- ✅ User
- ✅ Role
- ✅ Category
- ✅ Product
- ✅ Order
- ✅ OrderDetail
- ✅ Cart
- ✅ CartItem

### Bảng Admin (theo ADMIN_BLUEPRINT)
- ✅ ProductVariant
- ✅ Payment
- ✅ Refund
- ✅ Shipment
- ✅ Return
- ✅ InventoryTransaction
- ✅ Promotion
- ✅ Coupon
- ✅ CouponUsage
- ✅ Permission
- ✅ RolePermission
- ✅ AuditLog
- ✅ Address
- ✅ Media
- ✅ Collection
- ✅ ProductCollection

### Bảng Reviews & Comments
- ✅ ProductReview
- ✅ ReviewImage
- ✅ ReviewComment
- ✅ ReviewHelpful
- ✅ ReviewReport

## Dữ liệu mẫu được seed tự động

### 1. Roles
- **Admin**: Quản trị viên
- **User**: Người dùng

### 2. Users
- **admin**: Username: `admin`, Password: `Admin@123`
- **testuser**: Username: `testuser`, Password: `Test@123`

### 3. Categories
- Áo Nam
- Áo Nữ
- Quần Nam
- Quần Nữ
- Váy

### 4. Products
- 5 sản phẩm mẫu với hình ảnh và mô tả

### 5. Permissions
- Product.View, Product.Create, Product.Edit, Product.Delete
- Order.View, Order.Edit
- Review.Manage, Review.Approve, Review.Delete

### 6. RolePermissions
- Admin role được gán tất cả các quyền

### 7. Sample Reviews
- 3 đánh giá mẫu cho sản phẩm
- 1 bình luận mẫu (admin reply)

## Cách reset database

### Cách 1: Xóa database trong SQL Server Management Studio

1. Mở **SQL Server Management Studio**
2. Kết nối đến SQL Server
3. Expand **Databases**
4. Click phải vào database `FashionStore` → **Delete**
5. Chọn **Close existing connections** → **OK**
6. Chạy lại ứng dụng → Database sẽ được tạo lại tự động

### Cách 2: Sử dụng script SQL

```sql
USE master;
GO

-- Đóng tất cả connections
ALTER DATABASE FashionStore SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- Xóa database
DROP DATABASE FashionStore;
GO

-- Tạo lại database (rỗng)
CREATE DATABASE FashionStore;
GO
```

Sau đó chạy lại ứng dụng, database sẽ được tạo với đầy đủ bảng và dữ liệu mẫu.

### Cách 3: Sử dụng Package Manager Console (nếu có Entity Framework Migrations)

```powershell
# Xóa database
Drop-Database -Force

# Tạo lại database
Update-Database
```

## Kiểm tra database đã được tạo

### Kiểm tra trong SQL Server Management Studio

```sql
-- Xem tất cả các bảng
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Kiểm tra số lượng bảng (nên có khoảng 30+ bảng)
SELECT COUNT(*) AS TotalTables
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';

-- Kiểm tra dữ liệu đã được seed
SELECT COUNT(*) AS TotalUsers FROM [User];
SELECT COUNT(*) AS TotalRoles FROM [Role];
SELECT COUNT(*) AS TotalCategories FROM Category;
SELECT COUNT(*) AS TotalProducts FROM Product;
SELECT COUNT(*) AS TotalPermissions FROM Permission;
SELECT COUNT(*) AS TotalReviews FROM ProductReview;
```

### Kiểm tra trong ứng dụng

1. Chạy ứng dụng
2. Đăng nhập với:
   - Username: `admin`
   - Password: `Admin@123`
3. Kiểm tra các chức năng hoạt động bình thường

## Lưu ý quan trọng

### 1. Connection String

Đảm bảo `Web.config` có connection string đúng:

```xml
<connectionStrings>
  <add name="DefaultConnection" 
       connectionString="Data Source=localhost;Initial Catalog=FashionStore;Integrated Security=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 2. Quyền SQL Server

User SQL Server cần có quyền:
- `CREATE DATABASE`
- `CREATE TABLE`
- `CREATE INDEX`
- `ALTER TABLE`

### 3. Xóa database khi đang chạy

Nếu ứng dụng đang chạy và có kết nối đến database, cần:
- Dừng ứng dụng trước
- Hoặc đóng tất cả connections (ALTER DATABASE ... SET SINGLE_USER)

### 4. Dữ liệu sẽ bị mất

⚠️ **CẢNH BÁO**: Khi xóa database, tất cả dữ liệu sẽ bị mất. Chỉ làm điều này trong môi trường development.

## Troubleshooting

### Lỗi: "Cannot create database because it already exists"

**Giải pháp**: Xóa database trước khi chạy lại ứng dụng.

### Lỗi: "Cannot drop database because it is currently in use"

**Giải pháp**: 
```sql
ALTER DATABASE FashionStore SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE FashionStore;
```

### Lỗi: "Login failed for user"

**Giải pháp**: Kiểm tra connection string và quyền truy cập SQL Server.

### Database không tự động tạo

**Giải pháp**: 
1. Kiểm tra `Database.SetInitializer` đã được gọi trong constructor của `ApplicationDbContext`
2. Đảm bảo không có migration nào đang chạy
3. Thử xóa `bin` và `obj` folders, rebuild project

## Tùy chỉnh Seed Data

Nếu muốn thêm dữ liệu mẫu khác, chỉnh sửa method `Seed()` trong `DatabaseInitializer.cs`:

```csharp
protected override void Seed(ApplicationDbContext context)
{
    // Thêm dữ liệu mẫu của bạn ở đây
    // ...
    
    context.SaveChanges();
    base.Seed(context);
}
```

## Kết luận

Với cấu hình hiện tại, bạn chỉ cần:
1. ✅ Xóa database
2. ✅ Chạy lại ứng dụng
3. ✅ Tất cả bảng và dữ liệu mẫu sẽ được tạo tự động

Không cần chạy migration script thủ công! 🎉

