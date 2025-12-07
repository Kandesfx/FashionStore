# SỬA LỖI PACKAGES - CÁCH CUỐI CÙNG

## Vấn đề
Packages đã được download nhưng Visual Studio không nhận diện được references. Có thể do:
1. Version mismatch giữa Unity.Container và Unity.Mvc
2. Project file chưa được reload
3. Packages chưa được restore đúng cách

## Giải pháp - Cài đặt lại packages từ đầu

### Bước 1: Xóa packages hiện tại

1. **Đóng Visual Studio**
2. **Xóa thư mục** `packages/Unity.Container.5.11.11/` (version sai)
3. **Giữ lại** `packages/Unity.Mvc.5.11.1/` và các packages khác

### Bước 2: Cài đặt lại Unity.Container đúng version

**Trong Visual Studio:**

1. **Mở Package Manager Console**:
   - `Tools` → `NuGet Package Manager` → `Package Manager Console`

2. **Gỡ package cũ** (nếu có):
   ```powershell
   Uninstall-Package Unity.Container -Force
   ```

3. **Cài đặt lại đúng version**:
   ```powershell
   Install-Package Unity.Container -Version 5.11.1
   ```

### Bước 3: Restore tất cả packages

```powershell
Update-Package -reinstall
```

### Bước 4: Unload/Reload Project

1. **Click phải** vào project `FashionStore` → `Unload Project`
2. **Click phải lại** → `Reload Project`

### Bước 5: Clean và Rebuild

1. `Build` → `Clean Solution`
2. `Build` → `Rebuild Solution`

## Hoặc: Sử dụng Package Manager UI

1. **Click phải** vào Solution → `Manage NuGet Packages for Solution...`
2. **Tab Installed** → Tìm `Unity.Container`
3. **Uninstall** nếu có version 5.11.11
4. **Tab Browse** → Tìm `Unity.Container`
5. **Chọn version 5.11.1** → **Install**
6. **Restore** tất cả packages

## Kiểm tra sau khi cài

Sau khi cài xong, kiểm tra:
- `packages/Unity.Container.5.11.1/lib/net45/Unity.Container.dll` phải tồn tại
- `packages/Unity.Mvc.5.11.1/lib/net45/Unity.Mvc.dll` phải tồn tại
- `packages/Microsoft.AspNet.WebApi.Core.5.2.9/lib/net45/System.Web.Http.dll` phải tồn tại

## Nếu vẫn lỗi - Thử cách này

1. **Đóng Visual Studio**
2. **Xóa**:
   - `FashionStore/bin/`
   - `FashionStore/obj/`
   - `packages/Unity.Container.5.11.11/` (nếu có)
3. **Mở lại Visual Studio**
4. **Restore packages**: Click phải Solution → `Restore NuGet Packages`
5. **Rebuild**

## Lưu ý

- Unity.Container phải là version **5.11.1** (không phải 5.11.11)
- Unity.Mvc cần Unity.Container 5.11.1 làm dependency
- Sau khi cài xong, phải **Unload/Reload project** để Visual Studio nhận diện references mới

