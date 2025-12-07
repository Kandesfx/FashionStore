# SỬA LỖI SAU KHI INSTALL PACKAGE

## Vấn đề
Sau khi install package, xuất hiện rất nhiều lỗi về missing references. Điều này thường xảy ra khi:
1. Visual Studio chưa load lại project sau khi install package
2. Packages chưa được restore đúng cách
3. Project file bị cache

## Giải pháp

### Bước 1: Unload và Reload Project (QUAN TRỌNG)

1. **Trong Visual Studio**, click phải vào project `FashionStore` trong Solution Explorer
2. **Chọn** `Unload Project`
3. **Click phải lại** vào project (bây giờ sẽ hiện "Reload Project")
4. **Chọn** `Reload Project`
5. **Đợi** Visual Studio load lại project

### Bước 2: Clean và Rebuild

1. **Build** → `Clean Solution`
2. **Build** → `Rebuild Solution`

### Bước 3: Restore Packages lại

1. **Click phải** vào Solution → `Restore NuGet Packages`
2. **Đợi** quá trình restore hoàn tất

### Bước 4: Nếu vẫn lỗi - Xóa cache

1. **Đóng Visual Studio**
2. **Xóa các thư mục**:
   - `FashionStore/bin/`
   - `FashionStore/obj/`
   - `%LocalAppData%\Microsoft\VisualStudio\*\ComponentModelCache` (nếu có)
3. **Mở lại Visual Studio**
4. **Restore packages** và **Rebuild**

## Lưu ý về Unity.Mvc

Tôi đã sửa file `UnityConfig.cs` để dùng namespace đúng:
- **Trước**: `using Unity.Mvc5;`
- **Sau**: `using Unity.Mvc;`

Package Unity.Mvc.5.11.1 sử dụng namespace `Unity.Mvc` chứ không phải `Unity.Mvc5`.

## Kiểm tra sau khi sửa

Sau khi reload project và rebuild, các lỗi về:
- ✅ System references (System, System.Core, System.Web, etc.) - sẽ tự động resolve
- ✅ Unity.Mvc5 namespace - đã sửa thành Unity.Mvc
- ✅ Web API types - sẽ resolve sau khi packages được restore

## Nếu vẫn còn lỗi

Nếu sau khi làm các bước trên mà vẫn còn lỗi, hãy:
1. Kiểm tra Error List xem lỗi cụ thể là gì
2. Báo lại lỗi cụ thể để tôi có thể sửa tiếp

