# HƯỚNG DẪN CÀI ĐẶT PACKAGES CHO TEMPLATE PROJECT

## Cách 1: Sử dụng Package Manager Console (KHUYẾN NGHỊ)

### Bước 1: Mở Package Manager Console

1. **Visual Studio** → `Tools` → `NuGet Package Manager` → `Package Manager Console`

### Bước 2: Chạy các lệnh sau (copy từng dòng hoặc copy tất cả)

```powershell
# Core ASP.NET MVC
Install-Package Microsoft.AspNet.Mvc -Version 5.2.9 -ProjectName FashionStore
Install-Package Microsoft.AspNet.Razor -Version 3.2.9 -ProjectName FashionStore
Install-Package Microsoft.AspNet.WebPages -Version 3.2.9 -ProjectName FashionStore
Install-Package Microsoft.AspNet.Web.Optimization -Version 1.1.3 -ProjectName FashionStore
Install-Package Microsoft.Web.Infrastructure -Version 2.0.0 -ProjectName FashionStore
Install-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform -Version 2.0.1 -ProjectName FashionStore

# Web API
Install-Package Microsoft.AspNet.WebApi -Version 5.2.9 -ProjectName FashionStore
Install-Package Microsoft.AspNet.WebApi.WebHost -Version 5.2.9 -ProjectName FashionStore
Install-Package Microsoft.AspNet.WebApi.Core -Version 5.2.9 -ProjectName FashionStore
Install-Package Microsoft.AspNet.WebApi.Client -Version 5.2.9 -ProjectName FashionStore
Install-Package System.Net.Http -Version 4.3.4 -ProjectName FashionStore

# Unity (Dependency Injection)
Install-Package Unity.Mvc -Version 5.11.1 -ProjectName FashionStore
Install-Package Unity.Container -Version 5.11.11 -ProjectName FashionStore
Install-Package Unity.Abstractions -Version 5.11.7 -ProjectName FashionStore
Install-Package WebActivatorEx -Version 2.2.0 -ProjectName FashionStore

# Entity Framework
Install-Package EntityFramework -Version 6.4.4 -ProjectName FashionStore

# Additional packages
Install-Package BCrypt.Net-Next -Version 4.0.3 -ProjectName FashionStore
Install-Package PagedList.Mvc -Version 4.5.0 -ProjectName FashionStore
Install-Package NLog -Version 4.7.15 -ProjectName FashionStore
Install-Package NLog.Web -Version 4.9.3 -ProjectName FashionStore
Install-Package Newtonsoft.Json -Version 6.0.4 -ProjectName FashionStore

# Dependencies
Install-Package System.Runtime.CompilerServices.Unsafe -Version 4.5.3 -ProjectName FashionStore
Install-Package System.Threading.Tasks.Extensions -Version 4.5.2 -ProjectName FashionStore
```

### Bước 3: Sau khi cài xong

1. **Unload Project**: Click phải project → `Unload Project`
2. **Reload Project**: Click phải lại → `Reload Project`
3. **Clean Solution**: `Build` → `Clean Solution`
4. **Rebuild Solution**: `Build` → `Rebuild Solution`

## Cách 2: Sử dụng PowerShell Script

1. **Mở PowerShell** (Run as Administrator)
2. **Navigate** đến thư mục project:
   ```powershell
   cd D:\Hai\study\LTW\FashionStore
   ```
3. **Chạy script**:
   ```powershell
   .\install-packages.ps1
   ```

## Cách 3: Sử dụng Package Manager UI

1. **Click phải** vào Solution → `Manage NuGet Packages for Solution...`
2. **Tab Browse** → Tìm từng package và cài đặt với version tương ứng
3. **Lưu ý**: Cài đặt theo thứ tự:
   - Core MVC packages trước
   - Web API packages
   - Unity packages
   - Entity Framework
   - Additional packages

## Danh sách Packages cần cài

### Core ASP.NET MVC (6 packages)
- Microsoft.AspNet.Mvc (5.2.9)
- Microsoft.AspNet.Razor (3.2.9)
- Microsoft.AspNet.WebPages (3.2.9)
- Microsoft.AspNet.Web.Optimization (1.1.3)
- Microsoft.Web.Infrastructure (2.0.0)
- Microsoft.CodeDom.Providers.DotNetCompilerPlatform (2.0.1)

### Web API (5 packages)
- Microsoft.AspNet.WebApi (5.2.9)
- Microsoft.AspNet.WebApi.WebHost (5.2.9)
- Microsoft.AspNet.WebApi.Core (5.2.9)
- Microsoft.AspNet.WebApi.Client (5.2.9)
- System.Net.Http (4.3.4)

### Unity (4 packages)
- Unity.Mvc (5.11.1)
- Unity.Container (5.11.11)
- Unity.Abstractions (5.11.7)
- WebActivatorEx (2.2.0)

### Entity Framework (1 package)
- EntityFramework (6.4.4)

### Additional (5 packages)
- BCrypt.Net-Next (4.0.3)
- PagedList.Mvc (4.5.0)
- NLog (4.7.15)
- NLog.Web (4.9.3)
- Newtonsoft.Json (6.0.4)

### Dependencies (2 packages)
- System.Runtime.CompilerServices.Unsafe (4.5.3)
- System.Threading.Tasks.Extensions (4.5.2)

**Tổng cộng: 23 packages**

## Lưu ý

1. **Đảm bảo có kết nối internet** khi cài đặt
2. **Kiên nhẫn** - quá trình cài đặt có thể mất 5-10 phút
3. **Sau khi cài xong**, bắt buộc phải **Unload/Reload project**
4. **Clean và Rebuild** sau khi reload

## Kiểm tra sau khi cài

Sau khi cài xong, kiểm tra:
- ✅ File `packages.config` có đầy đủ 23 packages
- ✅ Thư mục `packages/` có đầy đủ các package folders
- ✅ Build project không còn lỗi về missing references

