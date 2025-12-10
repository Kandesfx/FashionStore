# HƯỚNG DẪN TRUY CẬP TRANG ADMIN

## Thông tin đăng nhập Admin

Sau khi database được khởi tạo, hệ thống sẽ tự động tạo tài khoản admin mặc định:

- **Username:** `admin`
- **Password:** `Admin@123`
- **Email:** `admin@fashionstore.com`

## Các bước truy cập Admin Panel

### Bước 1: Đăng nhập
1. Truy cập trang web: `http://localhost:[port]/Account/Login`
2. Nhập thông tin đăng nhập:
   - Username: `admin`
   - Password: `Admin@123`
3. Click nút "Đăng nhập"

### Bước 2: Truy cập Admin Dashboard
Sau khi đăng nhập thành công với tài khoản Admin, bạn có thể truy cập admin panel bằng các cách sau:

**Cách 1: Truy cập trực tiếp qua URL**
- Dashboard: `http://localhost:[port]/AdminDashboard`
- Quản lý sản phẩm: `http://localhost:[port]/AdminProduct`
- Quản lý đơn hàng: `http://localhost:[port]/AdminOrder`
- Quản lý người dùng: `http://localhost:[port]/AdminUser`

**Cách 2: Thêm link vào menu (nếu cần)**
Bạn có thể thêm link vào menu chính để dễ truy cập hơn.

## Các chức năng Admin Panel

### 1. Dashboard (`/AdminDashboard`)
- Xem thống kê tổng quan: Tổng đơn hàng, Doanh thu, Đơn chờ xử lý, Đơn hoàn thành
- Xem danh sách đơn hàng gần đây
- Xem tổng số sản phẩm và người dùng

### 2. Quản lý Sản phẩm (`/AdminProduct`)
- **Index:** Xem danh sách tất cả sản phẩm
- **Create:** Thêm sản phẩm mới
- **Edit:** Sửa thông tin sản phẩm
- **Details:** Xem chi tiết sản phẩm
- **Delete:** Xóa/vô hiệu hóa sản phẩm

### 3. Quản lý Đơn hàng (`/AdminOrder`)
- **Index:** Xem danh sách đơn hàng, lọc theo trạng thái
- **Details:** Xem chi tiết đơn hàng
- **Update Status:** Cập nhật trạng thái đơn hàng (Pending, Processing, Shipped, Delivered, Cancelled)

### 4. Quản lý Người dùng (`/AdminUser`)
- **Index:** Xem danh sách tất cả người dùng
- **Details:** Xem chi tiết người dùng
- **Edit:** Sửa thông tin người dùng, thay đổi vai trò
- **Delete:** Vô hiệu hóa người dùng

## Lưu ý quan trọng

### Bảo mật
- Chỉ người dùng có vai trò "Admin" mới có thể truy cập admin panel
- Nếu người dùng thường cố gắng truy cập, họ sẽ thấy trang "Không có quyền truy cập"
- Session sẽ được kiểm tra mỗi khi truy cập admin panel

### Xử lý lỗi
- Nếu chưa đăng nhập: Sẽ redirect về trang Login
- Nếu không có quyền Admin: Sẽ hiển thị trang Unauthorized
- Nếu có lỗi trong quá trình xử lý: Sẽ hiển thị thông báo lỗi

### Khởi tạo Database
Nếu database chưa được khởi tạo:
1. Chạy migration hoặc script SQL để tạo database
2. DatabaseInitializer sẽ tự động tạo:
   - 2 roles: Admin và User
   - 1 tài khoản admin mặc định
   - 5 categories mẫu
   - 5 sản phẩm mẫu

## Troubleshooting

### Không thể đăng nhập
1. Kiểm tra database đã được tạo chưa
2. Kiểm tra tài khoản admin đã được tạo chưa
3. Kiểm tra password có đúng không (phân biệt hoa thường)

### Không thể truy cập admin panel
1. Kiểm tra đã đăng nhập chưa
2. Kiểm tra session có Role = "Admin" không
3. Kiểm tra tài khoản có vai trò Admin không

### Lỗi khi truy cập dashboard
1. Kiểm tra database có dữ liệu không
2. Kiểm tra các service (OrderService, ProductService, UserService) có hoạt động không
3. Kiểm tra log để xem lỗi cụ thể

## Thay đổi mật khẩu Admin

Nếu muốn thay đổi mật khẩu admin, bạn có thể:
1. Đăng nhập vào admin panel
2. Vào Quản lý Người dùng
3. Tìm user "admin"
4. Sửa thông tin (lưu ý: cần implement chức năng đổi mật khẩu)

Hoặc thay đổi trực tiếp trong database:
```sql
UPDATE Users 
SET PasswordHash = [BCrypt hash của mật khẩu mới]
WHERE Username = 'admin'
```

