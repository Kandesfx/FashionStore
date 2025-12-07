# Script để xóa và tạo lại database FashionStore
# Chạy script này trong Package Manager Console hoặc PowerShell

Write-Host "========================================" -ForegroundColor Green
Write-Host "Reset Database FashionStore" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Kết nối đến LocalDB và xóa database
$connectionString = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True"

try {
    Write-Host "Đang kết nối đến LocalDB..." -ForegroundColor Yellow
    
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "Đang xóa database FashionStore (nếu tồn tại)..." -ForegroundColor Yellow
    
    $command = $connection.CreateCommand()
    $command.CommandText = @"
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'FashionStore')
BEGIN
    ALTER DATABASE [FashionStore] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [FashionStore];
    PRINT 'Database FashionStore đã được xóa';
END
ELSE
BEGIN
    PRINT 'Database FashionStore không tồn tại';
END
"@
    $command.ExecuteNonQuery()
    
    Write-Host "Database đã được xóa thành công!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Bây giờ hãy chạy lại ứng dụng để EF tự động tạo database mới." -ForegroundColor Cyan
    Write-Host "Database sẽ được tạo với dữ liệu mẫu từ DatabaseInitializer." -ForegroundColor Cyan
    
    $connection.Close()
}
catch {
    Write-Host "Lỗi: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Có thể database đang được sử dụng. Hãy:" -ForegroundColor Yellow
    Write-Host "1. Dừng IIS Express trong Visual Studio" -ForegroundColor Yellow
    Write-Host "2. Đóng tất cả kết nối đến database" -ForegroundColor Yellow
    Write-Host "3. Chạy lại script này" -ForegroundColor Yellow
}

