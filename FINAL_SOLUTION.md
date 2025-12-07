# GIẢI PHÁP CUỐI CÙNG - TẤT CẢ CÁC BƯỚC

## Tình trạng hiện tại
- ✅ Packages đã được download đầy đủ
- ✅ References trong .csproj đã được sửa
- ✅ UnityConfig.cs đã có đầy đủ using statements
- ✅ Web.config đã có binding redirects
- ❌ Visual Studio vẫn báo lỗi missing references

## Nguyên nhân
Visual Studio chưa load lại project và references sau khi thay đổi. Cần **Unload/Reload project**.

## GIẢI PHÁP - Làm theo thứ tự

### BƯỚC 1: Unload và Reload Project (BẮT BUỘC)

1. **Trong Visual Studio**
2. **Solution Explorer** → Click phải vào project `FashionStore`
3. **Chọn** `Unload Project`
4. **Click phải lại** vào project (sẽ hiện "Reload Project")
5. **Chọn** `Reload Project`
6. **Đợi** Visual Studio load lại (10-30 giây)

**QUAN TRỌNG**: Bước này BẮT BUỘC. Nếu bỏ qua, các lỗi sẽ không được sửa.

### BƯỚC 2: Clean Solution

1. **Build** → `Clean Solution`
2. **Đợi** quá trình clean hoàn tất

### BƯỚC 3: Rebuild Solution

1. **Build** → `Rebuild Solution` (Ctrl + Shift + B)
2. **Kiểm tra Error List**

## Nếu vẫn còn lỗi sau Bước 1-3

### BƯỚC 4: Xóa cache và restore lại

1. **Đóng Visual Studio**
2. **Xóa các thư mục**:
   ```
   FashionStore\bin\
   FashionStore\obj\
   ```
3. **Mở lại Visual Studio**
4. **Restore packages**: Click phải Solution → `Restore NuGet Packages`
5. **Unload/Reload project** (Bước 1)
6. **Rebuild** (Bước 3)

## Nếu vẫn lỗi Unity.Mvc namespace

### BƯỚC 5: Cài đặt lại Unity.Mvc

Trong **Package Manager Console**:

```powershell
Uninstall-Package Unity.Mvc -Force
Install-Package Unity.Mvc -Version 5.11.1
```

Sau đó:
1. **Unload/Reload project** (Bước 1)
2. **Rebuild** (Bước 3)

## Nếu vẫn lỗi Web API

### BƯỚC 6: Cài đặt lại Web API packages

Trong **Package Manager Console**:

```powershell
Update-Package Microsoft.AspNet.WebApi -Reinstall
Update-Package Microsoft.AspNet.WebApi.WebHost -Reinstall
Update-Package Microsoft.AspNet.WebApi.Core -Reinstall
```

Sau đó:
1. **Unload/Reload project** (Bước 1)
2. **Rebuild** (Bước 3)

## Kiểm tra packages

Đảm bảo các file sau tồn tại:

- ✅ `packages\Unity.Container.5.11.11\lib\net45\Unity.Container.dll`
- ✅ `packages\Unity.Mvc.5.11.1\lib\net45\Unity.Mvc.dll`
- ✅ `packages\Microsoft.AspNet.WebApi.Core.5.2.9\lib\net45\System.Web.Http.dll`
- ✅ `packages\Microsoft.AspNet.WebApi.WebHost.5.2.9\lib\net45\System.Web.Http.WebHost.dll`
- ✅ `packages\Microsoft.AspNet.WebApi.Client.5.2.9\lib\net45\System.Net.Http.Formatting.dll`

## Lưu ý quan trọng

1. **Unload/Reload project là BẮT BUỘC** sau mỗi lần thay đổi references
2. **Clean Solution** trước khi Rebuild
3. **Kiên nhẫn** - đợi Visual Studio load xong
4. Nếu vẫn lỗi, thử **đóng và mở lại Visual Studio**

## Thứ tự ưu tiên

1. **Bước 1-3** (Unload/Reload + Clean + Rebuild) - Thử trước
2. **Bước 4** (Xóa cache) - Nếu Bước 1-3 không được
3. **Bước 5-6** (Cài lại packages) - Nếu vẫn lỗi

**Hãy bắt đầu với Bước 1-3 ngay bây giờ!**

