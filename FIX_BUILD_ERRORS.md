# HƯỚNG DẪN SỬA LỖI BUILD

## Vấn đề
Các lỗi build xuất hiện vì **NuGet packages chưa được restore**. Tất cả các references đã được cấu hình đúng trong `.csproj`, nhưng các DLL files chưa được tải về.

## Giải pháp nhanh nhất

### Cách 1: Restore trong Visual Studio (KHUYẾN NGHỊ)

1. **Mở Visual Studio**
2. **Click phải vào Solution** `FashionStore` trong Solution Explorer
3. **Chọn** `Restore NuGet Packages`
4. **Đợi** quá trình restore hoàn tất (có thể mất 2-5 phút)
5. **Build lại**: `Build` → `Rebuild Solution`

### Cách 2: Sử dụng Package Manager Console

1. **Mở Package Manager Console**:
   - `Tools` → `NuGet Package Manager` → `Package Manager Console`

2. **Chạy lệnh**:
   ```powershell
   Update-Package -reinstall
   ```

3. **Hoặc restore từng package**:
   ```powershell
   Install-Package Microsoft.AspNet.Mvc -Version 5.2.9
   Install-Package Microsoft.AspNet.Web.Optimization -Version 1.1.3
   Install-Package Microsoft.AspNet.WebApi -Version 5.2.9
   Install-Package Microsoft.AspNet.WebApi.WebHost -Version 5.2.9
   Install-Package Unity.Mvc -Version 5.11.1
   Install-Package EntityFramework -Version 6.4.4
   Install-Package BCrypt.Net-Next -Version 4.0.3
   Install-Package System.Net.Http -Version 4.3.4
   ```

### Cách 3: Sử dụng Script (Nếu Visual Studio không restore được)

1. **Chạy file** `restore-packages.bat` (double-click)
2. **Hoặc chạy PowerShell script**:
   ```powershell
   .\restore-packages.ps1
   ```

### Cách 4: Xóa và restore lại (Nếu vẫn lỗi)

1. **Đóng Visual Studio**
2. **Xóa các thư mục**:
   - `packages/` (ở root solution)
   - `FashionStore/bin/`
   - `FashionStore/obj/`
3. **Mở lại Visual Studio**
4. **Restore packages** (Cách 1 hoặc 2)
5. **Build lại**

## Kiểm tra packages đã được restore

Sau khi restore, kiểm tra thư mục `packages/` có các folder sau:

```
packages/
├── Microsoft.AspNet.Mvc.5.2.9/
├── Microsoft.AspNet.Web.Optimization.1.1.3/
├── Microsoft.AspNet.WebApi.5.2.9/
├── Microsoft.AspNet.WebApi.Client.5.2.9/
├── Microsoft.AspNet.WebApi.Core.5.2.9/
├── Microsoft.AspNet.WebApi.WebHost.5.2.9/
├── Unity.Mvc.5.11.1/
├── EntityFramework.6.4.4/
├── BCrypt.Net-Next.4.0.3/
└── System.Net.Http.4.3.4/
```

## Các packages cần thiết

Dự án cần các packages sau (đã được khai báo trong `packages.config`):

- ✅ Microsoft.AspNet.Mvc (5.2.9)
- ✅ Microsoft.AspNet.Web.Optimization (1.1.3)
- ✅ Microsoft.AspNet.WebApi (5.2.9)
- ✅ Microsoft.AspNet.WebApi.Client (5.2.9)
- ✅ Microsoft.AspNet.WebApi.Core (5.2.9)
- ✅ Microsoft.AspNet.WebApi.WebHost (5.2.9)
- ✅ Unity.Mvc (5.11.1)
- ✅ EntityFramework (6.4.4)
- ✅ BCrypt.Net-Next (4.0.3)
- ✅ System.Net.Http (4.3.4)
- ✅ PagedList.Mvc (4.5.0)
- ✅ NLog (4.7.15)
- ✅ NLog.Web (4.9.3)

## Lưu ý quan trọng

1. **Đảm bảo có kết nối internet** khi restore
2. **Kiên nhẫn** - lần đầu restore có thể mất vài phút
3. **Kiểm tra NuGet Package Source**:
   - `Tools` → `Options` → `NuGet Package Manager` → `Package Sources`
   - Đảm bảo `nuget.org` được enable

## Sau khi restore thành công

1. **Build Solution**: `Ctrl + Shift + B`
2. **Kiểm tra Error List** - không còn lỗi về missing references
3. **Chạy project**: `F5`

## Nếu vẫn còn lỗi

Nếu sau khi restore packages mà vẫn còn lỗi, có thể do:

1. **Version conflict**: Thử xóa `packages.config` và restore lại
2. **Cache issues**: Xóa `%LocalAppData%\NuGet\Cache`
3. **Visual Studio cache**: Xóa `%LocalAppData%\Microsoft\VisualStudio\*\ComponentModelCache`

Hãy thử các cách trên và báo lại kết quả!

