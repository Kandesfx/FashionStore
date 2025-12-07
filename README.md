# FashionStore - Hệ thống bán quần áo trực tuyến

## Mô tả

FashionStore là một hệ thống thương mại điện tử (E-commerce) chuyên bán quần áo trực tuyến, được xây dựng trên nền tảng ASP.NET MVC với Entity Framework Code First.

## Yêu cầu hệ thống

- Visual Studio 2019 trở lên
- .NET Framework 4.7.2
- SQL Server LocalDB hoặc SQL Server Express
- NuGet Package Manager

## Cài đặt và Chạy Project

### Bước 1: Cài đặt NuGet Packages

Mở Package Manager Console trong Visual Studio và chạy lệnh:

```powershell
Update-Package -reinstall
```

Hoặc restore packages tự động khi build project.

### Bước 2: Cấu hình Database

Connection string đã được cấu hình trong `Web.config`:

```xml
<connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=FashionStore;Integrated Security=True;MultipleActiveResultSets=True" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

Database sẽ được tạo tự động khi chạy project lần đầu tiên với seed data mẫu.

### Bước 3: Build và Chạy

1. Build solution (Ctrl + Shift + B)
2. Chạy project (F5)
3. Trình duyệt sẽ mở tại `https://localhost:44330/`

### Bước 4: Đăng nhập Admin

Sau khi database được tạo, bạn có thể đăng nhập với tài khoản admin:

- **Username:** admin
- **Password:** Admin@123

## Cấu trúc Project

```
FashionStore/
├── Controllers/          # MVC Controllers
├── Models/
│   ├── Entities/        # Entity Models (Code First)
│   └── ViewModels/      # View Models
├── Views/               # Razor Views
├── Services/            # Business Logic Layer
│   ├── Interfaces/
│   └── Implementations/
├── Repositories/         # Data Access Layer
│   ├── Interfaces/
│   └── Implementations/
├── Data/                # DbContext và Database Initializer
├── Filters/             # Action Filters
├── Utilities/           # Helper classes
└── App_Start/           # Configuration files
```

## Tính năng chính

### Front-end (User)
- ✅ Trang chủ với sản phẩm nổi bật
- ✅ Danh sách sản phẩm với phân loại
- ✅ Tìm kiếm và lọc sản phẩm
- ✅ Chi tiết sản phẩm
- ✅ Giỏ hàng
- ✅ Đăng ký / Đăng nhập
- ✅ Quản lý thông tin cá nhân

### Back-end (Admin)
- ✅ Dashboard thống kê
- ✅ CRUD sản phẩm
- ✅ CRUD danh mục
- ✅ Quản lý đơn hàng
- ✅ Quản lý người dùng

### API
- ✅ RESTful API cho sản phẩm
- ✅ AJAX integration

## Công nghệ sử dụng

- **ASP.NET MVC 5.2.9**
- **Entity Framework 6.4.4** (Code First)
- **Bootstrap 5** (sẽ được cài đặt)
- **jQuery 3.6.0** (sẽ được cài đặt)
- **BCrypt.Net** (Password hashing)
- **Unity.Mvc** (Dependency Injection)
- **PagedList.Mvc** (Pagination)

## Tài liệu

Xem file `TAI_LIEU_KY_THUAT.md` để biết chi tiết về:
- Kiến trúc hệ thống
- Database design
- Implementation details
- Best practices

## Lưu ý

1. **Packages chưa được cài đặt**: Bạn cần restore NuGet packages trước khi build
2. **Database**: Database sẽ được tạo tự động khi chạy project lần đầu
3. **Seed Data**: Dữ liệu mẫu (admin user, categories, products) sẽ được tạo tự động
4. **Images**: Cần tạo thư mục `Content/images/products/` và thêm hình ảnh sản phẩm

## Phát triển tiếp theo

Các tính năng có thể thêm vào:
- [ ] Payment integration
- [ ] Email notifications
- [ ] Product reviews
- [ ] Wishlist
- [ ] Advanced search
- [ ] Multi-language support

## License

Dự án này được tạo cho mục đích học tập.

