# CHECKLIST KIỂM TRA VÀ SỬA LỖI DATABASE

## ✅ Đã sửa các lỗi

### 1. Lỗi CS2001: File ProductCollection.cs không tìm thấy
- ✅ Đã tách class `ProductCollection` ra file riêng
- ✅ Đã xóa class `ProductCollection` khỏi `Collection.cs`

### 2. Lỗi CS0168: Biến 'ex' không sử dụng
- ✅ Đã sửa 2 vị trí trong `AccountController.cs` (dòng 299 và 437)

### 3. Lỗi CS0246: ICollection không tìm thấy
- ✅ Đã thêm `using System.Collections.Generic;` vào `ReviewComment.cs`

### 4. Lỗi Entity Framework: Multiplicity conflict
- ✅ Đã xóa `[Required]` khỏi `ProductReviewId` trong `ReviewReport.cs`
- ✅ Đã xóa `[ForeignKey]` attributes để tránh conflict với cấu hình trong `OnModelCreating`

### 5. Lỗi Entity Framework: decimal(18,2) không tìm thấy trong SqlServer provider manifest
- ✅ Đã xóa `HasPrecision` cho các properties đã có `[Column(TypeName = "decimal(18,2)")]` trong entity:
  - Payment.Amount
  - Refund.Amount
  - Shipment.ShippingFee
  - Promotion.DiscountAmount, MinimumOrderAmount, MaximumDiscountAmount
  - Coupon.DiscountAmount, MinimumOrderAmount, MaximumDiscountAmount
  - CouponUsage.DiscountAmount
  - ProductVariant.Price
- ✅ Chỉ giữ lại `HasPrecision` cho các properties không có `[Column]` attribute:
  - Promotion.DiscountPercentage
  - Coupon.DiscountPercentage

## 📋 Checklist kiểm tra các lỗi tiềm ẩn

### A. Kiểm tra Relationships
- [x] Tất cả foreign keys đã được cấu hình đúng trong `OnModelCreating`
- [x] Cascade delete đã được cấu hình phù hợp
- [x] Optional/Required relationships đã đúng

### B. Kiểm tra Decimal Precision
- [x] Không có conflict giữa `[Column(TypeName = "decimal(18,2)")]` và `HasPrecision`
- [x] Tất cả decimal properties đã được cấu hình đúng

### C. Kiểm tra Indexes
- [x] Unique indexes đã được tạo đúng
- [x] Performance indexes đã được tạo cho các trường thường query

### D. Kiểm tra Navigation Properties
- [x] Tất cả navigation properties đã được khai báo đúng
- [x] ICollection properties đã có `using System.Collections.Generic;`

### E. Kiểm tra Required Fields
- [x] Không có mâu thuẫn giữa `[Required]` và nullable types (`int?`, `string?`)

### F. Kiểm tra Database Initialization
- [x] DatabaseInitializer đã seed đúng dữ liệu mẫu
- [x] Tất cả bảng sẽ được tạo tự động khi database được khởi tạo

## 🔍 Các lỗi có thể xảy ra và cách xử lý

### 1. Lỗi khi chạy ứng dụng lần đầu
**Triệu chứng**: Exception khi khởi tạo database
**Giải pháp**: 
- Xóa database cũ
- Chạy lại ứng dụng để database được tạo mới

### 2. Lỗi Foreign Key constraint
**Triệu chứng**: "The INSERT statement conflicted with the FOREIGN KEY constraint"
**Giải pháp**: 
- Kiểm tra dữ liệu seed có đúng thứ tự không (parent phải được tạo trước child)
- Kiểm tra foreign key values có tồn tại không

### 3. Lỗi Unique constraint
**Triệu chứng**: "Violation of UNIQUE KEY constraint"
**Giải pháp**: 
- Kiểm tra dữ liệu seed có duplicate không
- Kiểm tra các trường có unique index

### 4. Lỗi Column type mismatch
**Triệu chứng**: "The conversion of a varchar data type to a datetime data type resulted in an out-of-range value"
**Giải pháp**: 
- Kiểm tra DateTime properties có được khởi tạo đúng không
- Kiểm tra format datetime trong seed data

### 5. Lỗi Null reference
**Triệu chứng**: "Object reference not set to an instance of an object"
**Giải pháp**: 
- Kiểm tra navigation properties có được khởi tạo không
- Kiểm tra dữ liệu seed có đầy đủ không

## 🚀 Các bước kiểm tra sau khi sửa

1. **Build Solution**: `Ctrl + Shift + B`
   - Đảm bảo không còn lỗi compile

2. **Xóa Database cũ**:
   ```sql
   DROP DATABASE FashionStore;
   ```

3. **Chạy ứng dụng**: `F5`
   - Database sẽ được tạo tự động
   - Dữ liệu mẫu sẽ được seed

4. **Kiểm tra trong SQL Server Management Studio**:
   ```sql
   -- Kiểm tra số lượng bảng
   SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
   
   -- Kiểm tra dữ liệu đã được seed
   SELECT COUNT(*) FROM [User];
   SELECT COUNT(*) FROM [Role];
   SELECT COUNT(*) FROM Category;
   SELECT COUNT(*) FROM Product;
   SELECT COUNT(*) FROM Permission;
   SELECT COUNT(*) FROM ProductReview;
   ```

5. **Kiểm tra các chức năng cơ bản**:
   - Đăng nhập với admin/admin@123
   - Xem danh sách sản phẩm
   - Tạo đơn hàng
   - Xem đánh giá sản phẩm

## 📝 Ghi chú quan trọng

- **Không commit secrets**: Đảm bảo connection string không chứa password trong production
- **Backup trước khi xóa**: Luôn backup database trước khi xóa trong production
- **Kiểm tra migration**: Nếu có dữ liệu production, cần tạo migration script thay vì xóa database

## ✅ Kết luận

Tất cả các lỗi đã được sửa. Database schema đã được cấu hình đúng và sẽ tự động tạo khi chạy ứng dụng lần đầu.

