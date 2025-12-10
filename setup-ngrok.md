# Hướng dẫn setup Ngrok để IPN MoMo hoạt động với localhost

## Bước 1: Tải và cài đặt Ngrok

1. Truy cập: https://ngrok.com/download
2. Tải file ngrok.exe cho Windows
3. Giải nén vào thư mục bất kỳ (ví dụ: `C:\ngrok`)

## Bước 2: Đăng ký tài khoản Ngrok (miễn phí)

1. Truy cập: https://dashboard.ngrok.com/signup
2. Đăng ký tài khoản miễn phí
3. Lấy Authtoken từ dashboard: https://dashboard.ngrok.com/get-started/your-authtoken

## Bước 3: Cấu hình Ngrok

Mở Command Prompt hoặc PowerShell và chạy:

```bash
cd C:\ngrok
ngrok config add-authtoken YOUR_AUTHTOKEN_HERE
```

## Bước 4: Chạy Ngrok tunnel

1. Đảm bảo ứng dụng FashionStore đang chạy trên `https://localhost:44330`
2. Mở Command Prompt mới và chạy:

```bash
cd C:\ngrok
ngrok http 44330
```

3. Ngrok sẽ hiển thị URL công khai, ví dụ:
   ```
   Forwarding  https://abc123.ngrok-free.app -> https://localhost:44330
   ```

## Bước 5: Cập nhật Web.config

Cập nhật `Momo:RedirectUrl` và `Momo:IpnUrl` trong `Web.config`:

```xml
<add key="Momo:RedirectUrl" value="https://YOUR_NGROK_URL.ngrok-free.app/Payment/MomoReturn" />
<add key="Momo:IpnUrl" value="https://YOUR_NGROK_URL.ngrok-free.app/Payment/MomoIpn" />
```

**Lưu ý:**

- URL ngrok sẽ thay đổi mỗi lần chạy (trừ khi dùng tài khoản trả phí)
- Cần cập nhật lại Web.config mỗi khi restart ngrok
- Hoặc dùng tài khoản ngrok trả phí để có URL cố định

## Bước 6: Test

1. Khởi động lại ứng dụng FashionStore
2. Tạo đơn hàng và chọn thanh toán MoMo
3. Kiểm tra xem IPN có được gọi không (xem log trong Visual Studio)

## Lưu ý quan trọng

- Ngrok miễn phí có giới hạn số lượng request
- URL ngrok thay đổi mỗi lần restart (trừ tài khoản trả phí)
- Cần giữ ngrok chạy trong khi test
- Có thể dùng các công cụ khác: localtunnel, serveo, cloudflared

## Giải pháp thay thế (không cần ngrok)

Code hiện tại đã được cập nhật để xử lý qua redirect URL, không cần IPN.
Nhưng nếu muốn IPN hoạt động đầy đủ, cần deploy lên hosting thật.
