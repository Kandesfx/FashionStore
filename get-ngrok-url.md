# Cách lấy URL ngrok hiện tại

## Vấn đề: URL ngrok thay đổi mỗi lần chạy

Ngrok **miễn phí** sẽ tạo một URL **ngẫu nhiên** mỗi lần bạn chạy lệnh `ngrok http 44330`.

Ví dụ:

- Lần 1: `https://abc123.ngrok-free.app`
- Lần 2: `https://xyz789.ngrok-free.app`
- Lần 3: `https://erminia-nonobligated-nongracefully.ngrok-free.dev`

## Cách lấy URL ngrok hiện tại:

### Cách 1: Xem trong terminal ngrok

Khi bạn chạy `ngrok http 44330`, terminal sẽ hiển thị:

```
Forwarding  https://YOUR-URL-HERE.ngrok-free.dev -> http://localhost:44330
```

Copy URL này và cập nhật vào `Web.config`

### Cách 2: Xem trong ngrok web interface

1. Mở trình duyệt
2. Truy cập: `http://127.0.0.1:4040`
3. Bạn sẽ thấy URL hiện tại trong giao diện web

### Cách 3: Dùng API ngrok

Mở trình duyệt và truy cập:

```
http://127.0.0.1:4040/api/tunnels
```

Sẽ trả về JSON với URL hiện tại.

## Giải pháp:

### Option 1: Cập nhật Web.config mỗi lần chạy ngrok

Mỗi khi restart ngrok, bạn cần:

1. Lấy URL mới từ terminal
2. Cập nhật vào `Web.config`:
   ```xml
   <add key="Momo:RedirectUrl" value="https://YOUR-NEW-URL.ngrok-free.dev/Payment/MomoReturn" />
   <add key="Momo:IpnUrl" value="https://YOUR-NEW-URL.ngrok-free.dev/Payment/MomoIpn" />
   ```
3. Khởi động lại ứng dụng

### Option 2: Dùng ngrok với domain cố định (Trả phí)

Nếu muốn URL không đổi, bạn cần:

1. Đăng ký tài khoản ngrok trả phí
2. Mua domain cố định
3. Cấu hình domain trong ngrok dashboard

### Option 3: Tạo script tự động cập nhật (Khuyến nghị)

Tạo file `update-ngrok-config.ps1`:

```powershell
# Lấy URL từ ngrok API
$response = Invoke-RestMethod -Uri "http://127.0.0.1:4040/api/tunnels"
$ngrokUrl = $response.tunnels[0].public_url

# Cập nhật Web.config
$configPath = "FashionStore\Web.config"
$xml = [xml](Get-Content $configPath)
$xml.configuration.appSettings.add | Where-Object { $_.key -eq "Momo:RedirectUrl" } | ForEach-Object { $_.value = "$ngrokUrl/Payment/MomoReturn" }
$xml.configuration.appSettings.add | Where-Object { $_.key -eq "Momo:IpnUrl" } | ForEach-Object { $_.value = "$ngrokUrl/Payment/MomoIpn" }
$xml.Save($configPath)

Write-Host "Đã cập nhật Web.config với URL: $ngrokUrl"
```

## Lưu ý quan trọng:

⚠️ **Mỗi lần restart ngrok, URL sẽ thay đổi!**

✅ **Giải pháp tốt nhất:**

- Giữ ngrok chạy liên tục khi test
- Hoặc deploy lên hosting thật để có URL cố định
