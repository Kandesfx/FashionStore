# ADMIN BLUEPRINT – CỬA HÀNG TRỰC TUYẾN

Tài liệu này phác thảo hướng phát triển trang admin cho một cửa hàng chuyên nghiệp, tập trung vào vận hành, mở rộng, bảo mật và báo cáo. Có thể dùng làm khung để triển khai dần.

## 1. Mục tiêu & phạm vi
- Quản trị toàn bộ vận hành: sản phẩm, đơn hàng, tồn kho, khách hàng, khuyến mãi, nội dung, báo cáo.
- Tối ưu cho đội vận hành (CSKH, Kho, Marketing, Kế toán) với phân quyền rõ ràng.
- Hỗ trợ mở rộng: đa kho/chi nhánh, tích hợp thanh toán/vận chuyển, sàn TMĐT/POS.

## 2. Vai trò & phân quyền
- Vai trò gợi ý: Admin, CSKH, Kho, Marketing, Kế toán, ReadOnly.
- Quyền theo module & hành động: View / Create / Edit / Delete / Export / Approve / Refund.
- Audit log: ghi nhận thay đổi quan trọng (giá, tồn kho, trạng thái đơn, hoàn tiền).
- Bảo mật: 2FA cho admin, khóa sau nhiều lần đăng nhập sai, CAPTCHA, IP allowlist (tùy chọn).

## 3. Kiến trúc tổng quan
- API: REST/GraphQL, tách auth/permission, rate limit, logging, CSRF/XSS guard.
- Service layer: Sản phẩm, Đơn hàng, Tồn kho, Khách hàng, Khuyến mãi, Email/SMS, Báo cáo.
- Data: RDBMS (chuẩn hóa), cache (Redis) cho danh mục/sản phẩm, search (nếu cần).
- Config tách môi trường: Dev/Staging/Prod, secrets không commit.
- Log & giám sát: application log, error log, metric (tỉ lệ lỗi, thời gian phản hồi).

## 4. Module chức năng (Admin)
### 4.1 Dashboard
- KPI: doanh thu, số đơn theo trạng thái, top sản phẩm, tỉ lệ chuyển đổi giỏ, cảnh báo tồn kho.
- Bộ lọc thời gian, chi nhánh/kho; thông báo đơn mới, thanh toán lỗi, đổi trả.

### 4.2 Sản phẩm & danh mục
- CRUD sản phẩm, biến thể (size, màu), SKU/Barcode, giá thường/khuyến mãi, ảnh theo biến thể.
- Danh mục nhiều cấp, tag/collection, SEO (slug, meta).
- Tồn kho: nhập/xuất/điều chỉnh, cảnh báo tồn thấp, log thay đổi.
- Khuyến mãi/Flash sale/Combo; mã giảm giá theo điều kiện (danh mục, khách, số lượt).
- **Đánh giá & Bình luận**: Quản lý đánh giá sản phẩm, duyệt/từ chối, phản hồi, báo cáo spam.

### 4.3 Đơn hàng & vận chuyển
- Quy trình: tạo/duyệt/đóng gói/giao/hoàn thành/hủy; lý do hủy.
- Thanh toán: trạng thái, log giao dịch, hoàn tiền một phần/toàn phần.
- Vận chuyển: phí ship, hãng vận chuyển, mã vận đơn, trạng thái giao.
- Đổi trả/RMA: yêu cầu, duyệt, hoàn tiền/đổi sản phẩm.
- In ấn: phiếu giao hàng, tem vận chuyển, hóa đơn VAT.

### 4.4 Khách hàng & CRM nhẹ
- Hồ sơ: thông tin, địa chỉ, lịch sử đơn, LTV, kênh chuyển đổi.
- Phân khúc: khách mới/VIP/giỏ bỏ/hoàn hàng.
- Chăm sóc: ghi chú nội bộ, nhắc lịch, email/SMS kịch bản (giỏ bỏ, sau mua, sinh nhật).
- Loyalty (tùy chọn): tích điểm, đổi mã giảm giá.

### 4.5 Nội dung & Marketing
- Banner/Slider theo vị trí, lịch hiển thị.
- CMS cơ bản: blog/tin tức/chính sách.
- Landing page template cho chiến dịch.
- Mã giảm giá: %/số tiền, điều kiện, giới hạn, theo kênh/thiết bị.
- Email/SMS: template giao dịch, kịch bản theo sự kiện (đơn, reset password, giỏ bỏ).

### 4.6 Báo cáo & phân tích
- Bán hàng: doanh thu theo thời gian/kênh/sản phẩm/danh mục; biên lợi nhuận (nếu có giá vốn).
- Tồn kho: vòng quay, cảnh báo thấp/nhanh.
- Khách hàng: CLV, tần suất mua, tỉ lệ quay lại.
- Marketing: hiệu quả mã giảm giá/chiến dịch, nguồn traffic (UTM).
- Chuyển đổi giỏ: giỏ tạo -> đơn thành công, tỉ lệ bỏ giỏ.

### 4.7 Hệ thống & cấu hình
- Cấu hình thanh toán (cổng), vận chuyển (hãng), thuế/phí, email/SMS gateway.
- Quản lý môi trường, backup/restore, nhật ký hệ thống.
- Thông báo real-time (đơn mới, thanh toán lỗi, đổi trả).

### 4.8 Đánh giá & Bình luận sản phẩm (Reviews & Comments)
- **Quản lý đánh giá**: Xem danh sách, lọc theo trạng thái (Pending/Approved/Rejected/Reported), sản phẩm, rating.
- **Duyệt đánh giá**: Admin duyệt/từ chối đánh giá, ghi chú lý do, đánh dấu "Đã mua hàng" (verified purchase).
- **Phản hồi đánh giá**: Admin có thể phản hồi đánh giá của khách hàng, hỗ trợ giải đáp thắc mắc.
- **Quản lý bình luận**: Duyệt/từ chối bình luận, xóa bình luận không phù hợp, phản hồi bình luận.
- **Báo cáo & kiểm duyệt**: Xử lý báo cáo spam/inappropriate, tự động ẩn đánh giá có nhiều báo cáo.
- **Thống kê đánh giá**: Điểm trung bình, phân bố rating (1-5 sao), số lượng đánh giá theo thời gian.
- **Tính năng nâng cao**:
  - Đánh dấu "Hữu ích/Không hữu ích" cho đánh giá
  - Upload ảnh kèm đánh giá (tối đa 5 ảnh)
  - Lọc đánh giá theo: rating, verified purchase, có ảnh, hữu ích nhất
  - Tự động gửi email yêu cầu đánh giá sau khi giao hàng thành công
  - Hiển thị đánh giá nổi bật (top helpful, verified purchase) lên đầu
  - Phân tích sentiment đánh giá (tích cực/tiêu cực) - tùy chọn
  - Export danh sách đánh giá để phân tích

## 5. Luồng nghiệp vụ chính (tóm tắt)
- Đơn hàng: Web checkout → thanh toán → tạo đơn → duyệt/kho gói → giao → hoàn thành/hủy/hoàn tiền.
- Tồn kho: nhập kho → cập nhật tồn → cảnh báo → điều chỉnh.
- Khuyến mãi: tạo mã → điều kiện → áp dụng ở checkout → giới hạn lượt/thiết bị.
- Đổi trả: khách gửi yêu cầu → CS duyệt → kho nhận hàng → hoàn tiền/đổi.
- **Đánh giá sản phẩm**: Khách mua hàng → nhận email yêu cầu đánh giá → đánh giá (rating + text + ảnh) → chờ duyệt → Admin duyệt/từ chối → hiển thị công khai → khách khác vote hữu ích/bình luận → báo cáo nếu spam → Admin xử lý.

## 6. Dữ liệu & mô hình (gợi ý bảng)
- Products, Variants, Categories, Collections, Media.
- Promotions, Coupons.
- Orders, OrderItems, Payments, Refunds, Shipments, Returns/RMA.
- Customers, Addresses, Segments.
- InventoryTransactions (import/export/adjust).
- Roles, Permissions, AuditLogs.
- **Reviews & Comments**: ProductReviews, ReviewImages, ReviewComments, ReviewHelpfuls, ReviewReports.
- Logs (ứng dụng/giao dịch), Configs (cấu hình động).

### 6.1 Chi tiết bảng Reviews & Comments
- **ProductReview**: Id, ProductId, UserId, OrderId (để verify purchase), Rating (1-5), Title, ReviewText, Status (Pending/Approved/Rejected/Reported), IsVerifiedPurchase, HelpfulCount, NotHelpfulCount, ReportCount, AdminNotes, ReviewedByUserId, ReviewedDate, CreatedDate, UpdatedDate.
- **ReviewImage**: Id, ProductReviewId, ImageUrl, FileName, DisplayOrder, CreatedDate.
- **ReviewComment**: Id, ProductReviewId, UserId, CommentText, Status, IsAdminReply, ParentCommentId (để reply comment), ReportCount, CreatedDate, UpdatedDate.
- **ReviewHelpful**: Id, ProductReviewId, UserId, IsHelpful (true/false), CreatedDate. Unique constraint: (ProductReviewId, UserId).
- **ReviewReport**: Id, ProductReviewId (nullable), ReviewCommentId (nullable), UserId, ReportType (Spam/Inappropriate/Fake/Offensive/Other), Reason, Description, Status (Pending/Reviewed/Resolved/Dismissed), ReviewedByUserId, AdminNotes, ReviewedDate, CreatedDate.

## 7. Phiên bản & triển khai
- CI/CD: build, test, migrate DB, rollback nhanh.
- Migration: script up/down, seed roles và admin user.
- **Database Initialization**: Tự động tạo tất cả các bảng khi database được khởi tạo lần đầu (Entity Framework Code First), seed dữ liệu mẫu (roles, permissions, admin user, categories, products, sample reviews).
- Tách config per environment; không commit secrets.

## 8. Bảo mật & tuân thủ
- 2FA cho admin; khóa sau X lần login sai; CAPTCHA; IP allowlist (tùy chọn).
- CSRF, XSS, SQLi guard; HTTPS bắt buộc.
- Audit log cho hành động nhạy cảm; phân quyền chi tiết.
- Sao lưu định kỳ; kế hoạch khôi phục.

## 9. Hiệu năng & UX admin
- Lọc/tìm kiếm/sort mạnh; phân trang server-side.
- Bulk actions: cập nhật giá/tồn, đổi trạng thái đơn.
- Export/Import CSV/Excel (sản phẩm, khách, đơn).
- Thông báo real-time; trải nghiệm thao tác nhanh.

## 10. Lộ trình triển khai (ưu tiên)
1) Cốt lõi: Sản phẩm/biến thể, Đơn hàng/vận chuyển, Thanh toán, Tồn kho, Phân quyền + Audit log, Email giao dịch, Reset password, Báo cáo bán hàng cơ bản, **Đánh giá & Bình luận sản phẩm**.
2) Vận hành: Khuyến mãi/mã giảm, Banner/landing, CRM nhẹ + phân khúc, Cảnh báo tồn kho, **Quản lý đánh giá nâng cao (moderation, analytics)**.
3) Mở rộng: Loyalty, Đa kho/chi nhánh, Tích hợp sàn/POS, Thông báo real-time, Báo cáo nâng cao, **Sentiment analysis cho đánh giá**.

## 11. Kiểm thử
- Unit test: service (giá, khuyến mãi, tồn kho, đơn, **đánh giá, bình luận**).
- Integration test: thanh toán, vận chuyển, email/SMS, **email yêu cầu đánh giá, duyệt đánh giá**.
- Regression test: giỏ/checkout, mã giảm giá, phân quyền, **tạo/duyệt/xóa đánh giá**.
- Load test: danh sách sản phẩm/đơn lớn, dashboard, **danh sách đánh giá với nhiều bình luận**.

## 12. Tính năng bổ sung cho Reviews & Comments

### 12.1 Trải nghiệm người dùng (Frontend)
- **Hiển thị đánh giá**: 
  - Điểm trung bình và phân bố rating (1-5 sao) với thanh progress bar
  - Sắp xếp: Mới nhất, Hữu ích nhất, Rating cao/thấp, Có ảnh
  - Lọc: Verified purchase, Có ảnh, Rating cụ thể
  - Pagination cho danh sách đánh giá
- **Tạo đánh giá**:
  - Form đánh giá với rating stars (1-5), title, review text
  - Upload tối đa 5 ảnh (preview, crop, resize)
  - Hiển thị thông tin đơn hàng để verify purchase
  - Preview trước khi submit
- **Tương tác**:
  - Vote "Hữu ích/Không hữu ích" cho đánh giá
  - Bình luận đánh giá (reply comment)
  - Báo cáo đánh giá/bình luận không phù hợp
  - Share đánh giá lên mạng xã hội

### 12.2 Quản lý Admin (Backend)
- **Dashboard đánh giá**:
  - Tổng số đánh giá, đánh giá chờ duyệt, đánh giá bị báo cáo
  - Biểu đồ rating theo thời gian
  - Top sản phẩm được đánh giá nhiều nhất
  - Top đánh giá hữu ích nhất
- **Kiểm duyệt**:
  - Bulk actions: Duyệt/Từ chối nhiều đánh giá cùng lúc
  - Auto-moderation: Tự động từ chối đánh giá có từ khóa spam (cấu hình được)
  - Review queue: Danh sách đánh giá chờ duyệt, sắp xếp theo độ ưu tiên
- **Phân tích**:
  - Sentiment analysis: Phân tích cảm xúc đánh giá (tích cực/tiêu cực/trung tính)
  - Word cloud: Từ khóa xuất hiện nhiều nhất trong đánh giá
  - Trend analysis: Xu hướng đánh giá theo thời gian
  - Product comparison: So sánh rating giữa các sản phẩm

### 12.3 Tự động hóa
- **Email automation**:
  - Gửi email yêu cầu đánh giá sau 3-7 ngày kể từ khi giao hàng thành công
  - Reminder email nếu chưa đánh giá sau 14 ngày
  - Email cảm ơn sau khi đánh giá được duyệt
- **Moderation rules**:
  - Tự động approve đánh giá từ verified purchase với rating >= 4
  - Tự động reject đánh giá có từ khóa spam/offensive
  - Tự động ẩn đánh giá có ReportCount >= 5
- **Notifications**:
  - Thông báo real-time khi có đánh giá mới chờ duyệt
  - Thông báo khi đánh giá bị báo cáo
  - Thông báo khi rating trung bình sản phẩm thay đổi đáng kể

### 12.4 Tích hợp & API
- **API endpoints**:
  - GET /api/products/{id}/reviews (lấy danh sách đánh giá)
  - POST /api/products/{id}/reviews (tạo đánh giá)
  - PUT /api/reviews/{id} (cập nhật đánh giá của chính mình)
  - DELETE /api/reviews/{id} (xóa đánh giá của chính mình)
  - POST /api/reviews/{id}/helpful (vote hữu ích)
  - POST /api/reviews/{id}/comments (bình luận)
  - POST /api/reviews/{id}/report (báo cáo)
- **Export/Import**:
  - Export đánh giá ra CSV/Excel để phân tích
  - Import đánh giá từ file (cho migration dữ liệu)

---

Gợi ý bước tiếp theo: 
- ✅ **Đã hoàn thành**: Database schema cho Reviews & Comments, Database Initializer tự động tạo bảng
- 🔄 **Tiếp theo**: Tạo Repositories và Services cho Reviews, Controllers cho Admin và Frontend, Views hiển thị đánh giá

