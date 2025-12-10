# KIỂM TRA CHỨC NĂNG ADMIN PANEL

## ✅ Đã kiểm tra và sửa các vấn đề sau:

### 1. View Unauthorized
- ✅ Đã tạo view `Unauthorized.cshtml` để hiển thị khi người dùng không có quyền truy cập

### 2. AccountController
- ✅ Đã thêm method `Profile()` để tương thích với layout
- ✅ Giữ lại method `UserProfile()` để tương thích ngược

### 3. View Profile
- ✅ Đã sửa form action từ `UserProfile` sang `Profile`

### 4. OrderService
- ✅ Đã kiểm tra method `GetOrderStatistics()` - hoạt động bình thường

### 5. Authorization
- ✅ Tất cả controller admin đều có `[AuthorizeRole("Admin")]`
- ✅ Filter kiểm tra session và role đúng cách

## 🔍 Các URL Admin Panel:

1. **Dashboard:** `/AdminDashboard` hoặc `/AdminDashboard/Index`
2. **Quản lý Sản phẩm:** `/AdminProduct` hoặc `/AdminProduct/Index`
3. **Quản lý Đơn hàng:** `/AdminOrder` hoặc `/AdminOrder/Index`
4. **Quản lý Người dùng:** `/AdminUser` hoặc `/AdminUser/Index`

## 🔐 Thông tin đăng nhập Admin:

- **Username:** `admin`
- **Password:** `Admin@123`

## ⚠️ Các vấn đề cần lưu ý:

### 1. Database Initialization
- Đảm bảo database đã được khởi tạo
- DatabaseInitializer sẽ tự động tạo tài khoản admin khi database được tạo lần đầu

### 2. Session Management
- Session phải có các key: `UserId`, `Username`, `Role`, `FullName`
- Role phải là "Admin" (phân biệt hoa thường)

### 3. Routing
- Tất cả route admin đều sử dụng default routing
- Không cần cấu hình route đặc biệt

## 🧪 Cách kiểm tra:

### Bước 1: Kiểm tra Database
```sql
-- Kiểm tra role Admin
SELECT * FROM Roles WHERE RoleName = 'Admin'

-- Kiểm tra user admin
SELECT * FROM Users WHERE Username = 'admin'
```

### Bước 2: Kiểm tra đăng nhập
1. Truy cập: `/Account/Login`
2. Đăng nhập với: `admin` / `Admin@123`
3. Kiểm tra session có đầy đủ thông tin không

### Bước 3: Kiểm tra truy cập Admin
1. Sau khi đăng nhập, truy cập: `/AdminDashboard`
2. Nếu thành công: Sẽ thấy dashboard
3. Nếu thất bại: Sẽ redirect về Login hoặc hiển thị Unauthorized

### Bước 4: Kiểm tra các chức năng
- ✅ Dashboard: Hiển thị thống kê
- ✅ Product Index: Hiển thị danh sách sản phẩm
- ✅ Product Create: Form thêm sản phẩm
- ✅ Product Edit: Form sửa sản phẩm
- ✅ Order Index: Hiển thị danh sách đơn hàng
- ✅ Order Details: Hiển thị chi tiết đơn hàng
- ✅ User Index: Hiển thị danh sách người dùng

## 🐛 Xử lý lỗi thường gặp:

### Lỗi: "Không tìm thấy view"
- Kiểm tra tên view có đúng không
- Kiểm tra view có trong thư mục đúng không

### Lỗi: "NullReferenceException"
- Kiểm tra ViewBag có được set chưa
- Kiểm tra model có null không

### Lỗi: "Unauthorized"
- Kiểm tra session có Role = "Admin" không
- Kiểm tra user có vai trò Admin trong database không

### Lỗi: "Redirect về Login"
- Kiểm tra session có UserId không
- Kiểm tra session có bị timeout không

## 📝 Ghi chú:

- Tất cả controller admin đều sử dụng dependency injection
- Layout admin (`_AdminLayout.cshtml`) đã được tạo và sử dụng
- CSS admin (`admin.css`) đã được tạo và link trong layout
- Tất cả view admin đã được cập nhật để sử dụng layout mới

