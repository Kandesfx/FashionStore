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

## 5. Luồng nghiệp vụ chính (tóm tắt)
- Đơn hàng: Web checkout → thanh toán → tạo đơn → duyệt/kho gói → giao → hoàn thành/hủy/hoàn tiền.
- Tồn kho: nhập kho → cập nhật tồn → cảnh báo → điều chỉnh.
- Khuyến mãi: tạo mã → điều kiện → áp dụng ở checkout → giới hạn lượt/thiết bị.
- Đổi trả: khách gửi yêu cầu → CS duyệt → kho nhận hàng → hoàn tiền/đổi.

## 6. Dữ liệu & mô hình (gợi ý bảng)
- Products, Variants, Categories, Collections, Media.
- Promotions, Coupons.
- Orders, OrderItems, Payments, Refunds, Shipments, Returns/RMA.
- Customers, Addresses, Segments.
- InventoryTransactions (import/export/adjust).
- Roles, Permissions, AuditLogs.
- Logs (ứng dụng/giao dịch), Configs (cấu hình động).

## 7. Phiên bản & triển khai
- CI/CD: build, test, migrate DB, rollback nhanh.
- Migration: script up/down, seed roles và admin user.
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
1) Cốt lõi: Sản phẩm/biến thể, Đơn hàng/vận chuyển, Thanh toán, Tồn kho, Phân quyền + Audit log, Email giao dịch, Reset password, Báo cáo bán hàng cơ bản.
2) Vận hành: Khuyến mãi/mã giảm, Banner/landing, CRM nhẹ + phân khúc, Cảnh báo tồn kho.
3) Mở rộng: Loyalty, Đa kho/chi nhánh, Tích hợp sàn/POS, Thông báo real-time, Báo cáo nâng cao.

## 11. Kiểm thử
- Unit test: service (giá, khuyến mãi, tồn kho, đơn).
- Integration test: thanh toán, vận chuyển, email/SMS.
- Regression test: giỏ/checkout, mã giảm giá, phân quyền.
- Load test: danh sách sản phẩm/đơn lớn, dashboard.

---

Gợi ý bước tiếp theo: chọn module ưu tiên (ví dụ đơn hàng + sản phẩm + phân quyền) và thiết kế DB chi tiết cho các bảng chính. Bạn muốn mình đào sâu module nào trước?

