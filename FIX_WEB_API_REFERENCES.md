# SỬA LỖI WEB API REFERENCES

## Vấn đề
Web API packages đã được download nhưng Visual Studio không nhận diện:
- System.Web.Http không tìm thấy
- System.Web.Http.WebHost không tìm thấy  
- System.Net.Http.Formatting không tìm thấy

## Nguyên nhân
Visual Studio chưa load lại project sau khi thay đổi references hoặc packages chưa được restore đúng.

## GIẢI PHÁP

### Bước 1: Kiểm tra References trong .csproj

Mở file `FashionStore.csproj` và kiểm tra có các dòng sau:

```xml
<Reference Include="System.Web.Http, Version=5.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35">
  <HintPath>..\packages\Microsoft.AspNet.WebApi.Core.5.2.9\lib\net45\System.Web.Http.dll</HintPath>
</Reference>
<Reference Include="System.Web.Http.WebHost, Version=5.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35">
  <HintPath>..\packages\Microsoft.AspNet.WebApi.WebHost.5.2.9\lib\net45\System.Web.Http.WebHost.dll</HintPath>
</Reference>
<Reference Include="System.Net.Http.Formatting, Version=5.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35">
  <HintPath>..\packages\Microsoft.AspNet.WebApi.Client.5.2.9\lib\net45\System.Net.Http.Formatting.dll</HintPath>
</Reference>
```

### Bước 2: Unload và Reload Project (QUAN TRỌNG)

1. **Solution Explorer** → Click phải vào project `FashionStore`
2. **Chọn** `Unload Project`
3. **Click phải lại** → `Reload Project`
4. **Đợi** Visual Studio load lại

### Bước 3: Clean và Rebuild

1. `Build` → `Clean Solution`
2. `Build` → `Rebuild Solution`

### Bước 4: Nếu vẫn lỗi - Cài đặt lại Web API packages

Trong **Package Manager Console**:

```powershell
Uninstall-Package Microsoft.AspNet.WebApi -Force
Uninstall-Package Microsoft.AspNet.WebApi.WebHost -Force
Uninstall-Package Microsoft.AspNet.WebApi.Core -Force
Uninstall-Package Microsoft.AspNet.WebApi.Client -Force

Install-Package Microsoft.AspNet.WebApi -Version 5.2.9
Install-Package Microsoft.AspNet.WebApi.WebHost -Version 5.2.9
Install-Package Microsoft.AspNet.WebApi.Core -Version 5.2.9
Install-Package Microsoft.AspNet.WebApi.Client -Version 5.2.9
```

Sau đó:
1. **Unload/Reload project** (Bước 2)
2. **Rebuild** (Bước 3)

### Bước 5: Kiểm tra packages tồn tại

Đảm bảo các file sau tồn tại:

- ✅ `packages\Microsoft.AspNet.WebApi.Core.5.2.9\lib\net45\System.Web.Http.dll`
- ✅ `packages\Microsoft.AspNet.WebApi.WebHost.5.2.9\lib\net45\System.Web.Http.WebHost.dll`
- ✅ `packages\Microsoft.AspNet.WebApi.Client.5.2.9\lib\net45\System.Net.Http.Formatting.dll`

## Lưu ý

**Unload/Reload project là BẮT BUỘC** để Visual Studio nhận diện references mới.

