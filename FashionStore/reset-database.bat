@echo off
echo ========================================
echo Reset Database FashionStore
echo ========================================
echo.

echo Đang xóa database FashionStore...
echo.

sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "IF EXISTS (SELECT name FROM sys.databases WHERE name = 'FashionStore') BEGIN ALTER DATABASE [FashionStore] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [FashionStore]; PRINT 'Database FashionStore đã được xóa'; END ELSE BEGIN PRINT 'Database FashionStore không tồn tại'; END"

echo.
echo ========================================
echo Hoàn thành!
echo ========================================
echo.
echo Bây giờ hãy chạy lại ứng dụng trong Visual Studio
echo để EF tự động tạo database mới với cấu trúc đã cập nhật.
echo.
pause

