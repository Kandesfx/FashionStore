# HƯỚNG DẪN RESTORE NUGET PACKAGES

## Các bước để sửa lỗi build

### Bước 1: Xóa thư mục packages và obj (Nếu cần)

1. Đóng Visual Studio
2. Xóa thư mục `packages/` ở root của solution
3. Xóa thư mục `obj/` và `bin/` trong project FashionStore
4. Mở lại Visual Studio

### Bước 2: Restore NuGet Packages

**Cách 1: Tự động restore khi build**
1. Click phải vào Solution → `Restore NuGet Packages`
2. Hoặc Build Solution (Ctrl + Shift + B) - Visual Studio sẽ tự động restore

**Cách 2: Sử dụng Package Manager Console**
1. Tools → NuGet Package Manager → Package Manager Console
2. Chạy lệnh:
```powershell
Update-Package -reinstall
```

**Cách 3: Sử dụng NuGet Package Manager UI**
1. Click phải vào Solution → `Manage NuGet Packages for Solution...`
2. Chuyển sang tab `Installed`
3. Click `Restore` nếu có thông báo

### Bước 3: Kiểm tra các packages cần thiết

Sau khi restore, kiểm tra các packages sau có trong thư mục `packages/`:

- ✅ Microsoft.AspNet.Mvc.5.2.9
- ✅ Microsoft.AspNet.Web.Optimization.1.1.3
- ✅ Microsoft.AspNet.WebApi.5.2.9
- ✅ Microsoft.AspNet.WebApi.Client.5.2.9
- ✅ Microsoft.AspNet.WebApi.Core.5.2.9
- ✅ Microsoft.AspNet.WebApi.WebHost.5.2.9
- ✅ EntityFramework.6.4.4
- ✅ Unity.Mvc.5.11.1
- ✅ BCrypt.Net-Next.4.0.3
- ✅ System.Net.Http.4.3.4

### Bước 4: Build lại project

1. Build → Clean Solution
2. Build → Rebuild Solution
3. Kiểm tra Error List

## Nếu vẫn còn lỗi

### Lỗi về Unity packages
Nếu Unity packages không được restore, thử cài đặt lại:
```powershell
Install-Package Unity.Mvc -Version 5.11.1
```

### Lỗi về Web API packages
Thử cài đặt lại Web API:
```powershell
Install-Package Microsoft.AspNet.WebApi -Version 5.2.9
Install-Package Microsoft.AspNet.WebApi.WebHost -Version 5.2.9
```

### Lỗi về System.Web.Optimization
Thử cài đặt lại:
```powershell
Install-Package Microsoft.AspNet.Web.Optimization -Version 1.1.3
```

## Lưu ý quan trọng

1. **Đảm bảo có kết nối internet** khi restore packages
2. **Kiên nhẫn** - lần đầu restore có thể mất vài phút
3. **Kiểm tra NuGet Package Source** trong Visual Studio:
   - Tools → Options → NuGet Package Manager → Package Sources
   - Đảm bảo `nuget.org` được enable

## Sau khi restore thành công

1. Build Solution (Ctrl + Shift + B)
2. Kiểm tra Error List - nếu còn lỗi, hãy báo lại
3. Chạy project (F5) để test

