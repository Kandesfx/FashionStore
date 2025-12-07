# Hướng Dẫn Xóa Database và Tạo Lại

## Cách 1: Dùng SQL Server Management Studio (SSMS)

1. Mở **SQL Server Management Studio**
2. Kết nối đến: `(localdb)\MSSQLLocalDB`
3. Tìm database `FashionStore` trong Object Explorer
4. Click phải → **Delete**
5. Chọn "Close existing connections" → **OK**

## Cách 2: Dùng Visual Studio SQL Server Object Explorer

1. Trong Visual Studio: `View` → `SQL Server Object Explorer`
2. Mở rộng `(localdb)\MSSQLLocalDB` → `Databases`
3. Tìm `FashionStore` → Click phải → **Delete**
4. Xác nhận xóa

## Cách 3: Dùng Package Manager Console

```powershell
# Xóa database
sqllocaldb stop MSSQLLocalDB
sqllocaldb delete MSSQLLocalDB
sqllocaldb create MSSQLLocalDB
```

## Cách 4: Dùng script PowerShell (Đơn giản nhất)

Chạy file `reset-database.ps1` hoặc `reset-database.bat`

## Sau khi xóa database:

1. **Chạy lại ứng dụng** (F5)
2. EF sẽ tự động tạo database mới với cấu trúc đã cập nhật
3. Dữ liệu mẫu sẽ được seed tự động từ `DatabaseInitializer`

## Thông tin đăng nhập mặc định:

- **Username:** `admin`
- **Password:** `Admin@123`

