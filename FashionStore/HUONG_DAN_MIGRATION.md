# Hướng Dẫn Chạy Migration Entity Framework

## Bước 1: Enable Migrations

Mở **Package Manager Console** trong Visual Studio:
- `Tools` → `NuGet Package Manager` → `Package Manager Console`

Chạy lệnh:
```powershell
Enable-Migrations
```

## Bước 2: Tạo Migration đầu tiên

```powershell
Add-Migration InitialCreate
```

## Bước 3: Cập nhật database

```powershell
Update-Database
```

## Nếu đã có database và muốn tạo migration mới

```powershell
# Tạo migration mới cho các thay đổi
Add-Migration UpdateCartUserRelationship

# Cập nhật database
Update-Database
```

## Xóa tất cả migrations và tạo lại

```powershell
# Xóa tất cả migrations
Remove-Migration -Force

# Tạo migration mới
Add-Migration InitialCreate

# Cập nhật database
Update-Database
```

## Lưu ý

- Nếu gặp lỗi về existing database, có thể cần xóa database trước
- Migration sẽ tạo file trong thư mục `Migrations/`
- Luôn commit file migration vào source control

