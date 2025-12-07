-- Script tạo database FashionStore
-- Chạy script này trong SQL Server Management Studio hoặc sqlcmd

USE master;
GO

-- Xóa database nếu đã tồn tại
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'FashionStore')
BEGIN
    ALTER DATABASE [FashionStore] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [FashionStore];
    PRINT 'Database FashionStore đã được xóa';
END
GO

-- Tạo database mới
CREATE DATABASE [FashionStore]
GO

PRINT 'Database FashionStore đã được tạo thành công!';
PRINT 'Bây giờ hãy chạy ứng dụng để EF tự động tạo tables và seed dữ liệu.';
GO

