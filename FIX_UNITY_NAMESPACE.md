# SỬA LỖI UNITY.MVC NAMESPACE

## Vấn đề
Lỗi: `The type or namespace name 'Mvc' does not exist in the namespace 'Unity'`

## Nguyên nhân
Package Unity.Mvc.5.11.1 có thể không có namespace `Unity.Mvc` hoặc cần cài thêm package.

## Giải pháp

### Cách 1: Cài đặt lại Unity.Mvc package

Trong **Package Manager Console**:

```powershell
Uninstall-Package Unity.Mvc -Force
Install-Package Unity.Mvc -Version 5.11.1
```

### Cách 2: Kiểm tra và sửa namespace

Nếu package Unity.Mvc không có namespace `Unity.Mvc`, có thể cần:

1. **Kiểm tra** xem `UnityDependencyResolver` nằm ở namespace nào
2. **Sửa** `UnityConfig.cs` để dùng đúng namespace

### Cách 3: Dùng Unity.Container trực tiếp

Nếu Unity.Mvc không hoạt động, có thể dùng Unity.Container trực tiếp và tự tạo DependencyResolver.

## Thử ngay

1. **Unload/Reload project** (quan trọng!)
2. **Clean Solution**
3. **Rebuild Solution**

Nếu vẫn lỗi, hãy thử cài lại Unity.Mvc package.

