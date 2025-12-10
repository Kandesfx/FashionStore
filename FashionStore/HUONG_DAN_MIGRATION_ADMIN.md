# HƯỚNG DẪN MIGRATION DATABASE CHO ADMIN BLUEPRINT

## Tổng quan

Tài liệu này hướng dẫn cách thêm các bảng mới vào database theo ADMIN_BLUEPRINT để hỗ trợ hệ thống admin đầy đủ.

## Các bảng đã được thêm

### 1. Các bảng cốt lõi
- **ProductVariant**: Biến thể sản phẩm (size, màu, SKU, Barcode)
- **Payment**: Thanh toán đơn hàng
- **Refund**: Hoàn tiền
- **Shipment**: Vận chuyển đơn hàng
- **Return**: Đổi trả hàng (RMA)

### 2. Các bảng quản lý
- **InventoryTransaction**: Giao dịch tồn kho (nhập/xuất/điều chỉnh)
- **Promotion**: Khuyến mãi
- **Coupon**: Mã giảm giá
- **CouponUsage**: Lịch sử sử dụng mã giảm giá

### 3. Các bảng hệ thống
- **Permission**: Quyền truy cập
- **RolePermission**: Quan hệ Role-Permission
- **AuditLog**: Nhật ký audit
- **Address**: Địa chỉ khách hàng
- **Media**: Media (ảnh, video) của sản phẩm
- **Collection**: Bộ sưu tập sản phẩm
- **ProductCollection**: Quan hệ Product-Collection

### 4. Các trường đã cập nhật

#### Product
- Slug, MetaTitle, MetaDescription (SEO)
- SKU, Barcode
- CostPrice (giá vốn)
- LowStockThreshold (cảnh báo tồn thấp)
- UpdatedDate

#### Order
- RecipientPhone, RecipientName
- ApprovedDate, PackedDate, ShippedDate, CompletedDate, CancelledDate
- CancellationReason
- ShippingFee, DiscountAmount
- CouponCode
- UpdatedDate

#### User
- Gender, DateOfBirth
- ConversionChannel
- TotalOrderValue, TotalOrders
- InternalNotes
- UpdatedDate, LastLoginDate

#### Category
- ParentCategoryId (danh mục nhiều cấp)
- Slug, MetaTitle, MetaDescription (SEO)
- UpdatedDate

## Cách chạy Migration

### Cách 1: Sử dụng SQL Server Management Studio (SSMS)

1. Mở **SQL Server Management Studio**
2. Kết nối đến database `FashionStore`
3. Mở file `FashionStore/Migrations/001_Add_Admin_Tables.sql`
4. Chạy script (F5 hoặc Execute)

### Cách 2: Sử dụng Visual Studio

1. Mở **Server Explorer** trong Visual Studio
2. Kết nối đến database
3. Click phải vào database → **New Query**
4. Copy nội dung file `001_Add_Admin_Tables.sql` và paste vào query window
5. Execute query

### Cách 3: Sử dụng Command Line (sqlcmd)

```bash
sqlcmd -S localhost -d FashionStore -i "FashionStore/Migrations/001_Add_Admin_Tables.sql"
```

## Kiểm tra Migration

Sau khi chạy migration, kiểm tra các bảng đã được tạo:

```sql
-- Kiểm tra các bảng mới
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Kiểm tra các cột mới trong Product
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Product'
ORDER BY ORDINAL_POSITION;

-- Kiểm tra các foreign keys
SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTableName,
    COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferencedColumnName
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fc ON fk.object_id = fc.constraint_object_id
ORDER BY TableName, ForeignKeyName;
```

## Lưu ý quan trọng

1. **Backup database trước khi chạy migration**
   ```sql
   BACKUP DATABASE FashionStore TO DISK = 'C:\Backup\FashionStore_Before_Admin_Migration.bak'
   ```

2. **Kiểm tra tên database**: Đảm bảo tên database trong script SQL khớp với database thực tế của bạn.

3. **Kiểm tra constraints**: Script đã bao gồm các foreign key constraints. Nếu có lỗi, có thể do dữ liệu hiện tại không hợp lệ.

4. **Indexes**: Các index đã được tạo tự động để tối ưu hiệu năng truy vấn.

## Rollback (nếu cần)

Nếu cần rollback migration, chạy script sau (CHỈ KHI CẦN THIẾT):

```sql
-- XÓA CÁC BẢNG MỚI (CHỈ CHẠY KHI CẦN ROLLBACK)
DROP TABLE IF EXISTS [dbo].[ProductCollection];
DROP TABLE IF EXISTS [dbo].[Collection];
DROP TABLE IF EXISTS [dbo].[Media];
DROP TABLE IF EXISTS [dbo].[Address];
DROP TABLE IF EXISTS [dbo].[AuditLog];
DROP TABLE IF EXISTS [dbo].[RolePermission];
DROP TABLE IF EXISTS [dbo].[Permission];
DROP TABLE IF EXISTS [dbo].[CouponUsage];
DROP TABLE IF EXISTS [dbo].[Coupon];
DROP TABLE IF EXISTS [dbo].[Promotion];
DROP TABLE IF EXISTS [dbo].[InventoryTransaction];
DROP TABLE IF EXISTS [dbo].[Return];
DROP TABLE IF EXISTS [dbo].[Shipment];
DROP TABLE IF EXISTS [dbo].[Refund];
DROP TABLE IF EXISTS [dbo].[Payment];
DROP TABLE IF EXISTS [dbo].[ProductVariant];

-- XÓA CÁC CỘT MỚI (CHỈ CHẠY KHI CẦN ROLLBACK)
-- Lưu ý: Chỉ xóa nếu không có dữ liệu quan trọng
ALTER TABLE [dbo].[Category] DROP COLUMN IF EXISTS [ParentCategoryId], [Slug], [MetaTitle], [MetaDescription], [UpdatedDate];
ALTER TABLE [dbo].[User] DROP COLUMN IF EXISTS [Gender], [DateOfBirth], [ConversionChannel], [TotalOrderValue], [TotalOrders], [InternalNotes], [UpdatedDate], [LastLoginDate];
ALTER TABLE [dbo].[Order] DROP COLUMN IF EXISTS [RecipientPhone], [RecipientName], [ApprovedDate], [PackedDate], [ShippedDate], [CompletedDate], [CancelledDate], [CancellationReason], [ShippingFee], [DiscountAmount], [CouponCode], [UpdatedDate];
ALTER TABLE [dbo].[Product] DROP COLUMN IF EXISTS [Slug], [MetaTitle], [MetaDescription], [SKU], [Barcode], [CostPrice], [LowStockThreshold], [UpdatedDate];
```

## Bước tiếp theo

Sau khi migration thành công:

1. **Cập nhật Entity Framework**: Các entity đã được tạo trong `Models/Entities/`
2. **Cập nhật ApplicationDbContext**: Đã được cập nhật với các DbSet mới
3. **Tạo Repositories**: Tạo các repository cho các entity mới
4. **Tạo Services**: Tạo các service cho business logic
5. **Tạo Controllers**: Tạo các controller cho admin

## Hỗ trợ

Nếu gặp lỗi khi chạy migration, kiểm tra:
- Tên database có đúng không
- Có đủ quyền để tạo bảng không
- Có conflict với dữ liệu hiện tại không

