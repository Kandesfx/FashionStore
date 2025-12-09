## Hướng dẫn tích hợp MoMo (ASP.NET MVC)

### 1. Chuẩn bị

- Đăng ký merchant MoMo, lấy `partnerCode`, `accessKey`, `secretKey`.
- Endpoint product: `https://payment.momo.vn/v2/gateway/api/create`.
- Khai báo `redirectUrl` (MoMo redirect user) và `ipnUrl` (MoMo gọi server-server).
<!-- - Dùng sandbox test trước khi lên production. -->

### 2. Luồng tổng quan

1. User chọn “Chuyển khoản” → “MoMo” tại checkout.
2. Backend sinh `orderId`, `requestId`, `amount`, ký `signature` (HMAC-SHA256).
3. Gọi API `create` của MoMo → nhận `payUrl`.
4. Redirect user sang `payUrl`.
5. MoMo redirect user về `redirectUrl` và gọi IPN tới `ipnUrl`.
6. Backend verify chữ ký IPN, đối chiếu `amount`, cập nhật đơn hàng `Paid/Failed`.

<!-- payUrl:URL thanh toán MoMo trả về để bạn redirect
     ipnUrl:URL MoMo gửi thông báo thanh toán (IPN)
     redirectURL:URL MoMo sẽ đưa người dùng quay về sau khi thanh toán
     IPN(Instant Payment Notification):là một request mà MoMo gửi chủ động đến server của bạn để báo kết quả thanh toán.
 -->

### 3. Chuỗi ký (signature)

```
accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}
```

- `signature` = HMAC-SHA256(raw, `secretKey`), hex lowercase.

### 4. Payload tạo thanh toán (requestType: captureWallet)

```json
{
  "partnerCode": "...",
  "accessKey": "...",
  "requestId": "...",
  "amount": "100000",
  "orderId": "...",
  "orderInfo": "Thanh toan don ...",
  "redirectUrl": "https://yourdomain.com/Payment/MomoReturn",
  "ipnUrl": "https://yourdomain.com/Payment/MomoIpn",
  "extraData": "",
  "requestType": "captureWallet",
  "signature": "hmacsha256...",
  "lang": "vi"
}
```

MoMo trả về `payUrl`; redirect user tới URL này.

### 5. Code mẫu (pseudo C#)

```csharp
var raw = $"accessKey={cfg.AccessKey}&amount={amount}&extraData=&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={cfg.PartnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType=captureWallet";
var signature = SignHmacSha256(raw, cfg.SecretKey);
// POST payload JSON lên endpoint, đọc payUrl
```

Hàm ký:

```csharp
string SignHmacSha256(string text, string key) {
    using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
    return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "").ToLower();
}
```

### 6. Xử lý IPN (bắt buộc)

- MoMo POST JSON tới `ipnUrl`. Lấy các trường (orderId, requestId, amount, resultCode, message, extraData, signature...).
- Dựng lại `rawSignature` đúng thứ tự, HMAC-SHA256 với `secretKey`, so sánh với `signature`.
- Nếu hợp lệ và `resultCode == 0` **và** `amount` khớp đơn hàng → đánh dấu `Paid`; ngược lại `Failed`.
- Phản hồi MoMo:

```json
{
  "partnerCode": "...",
  "requestId": "...",
  "orderId": "...",
  "errorCode": 0,
  "message": "Successful"
}
```

### 7. Xử lý redirect cho user

- `redirectUrl` hiển thị kết quả thân thiện (đọc `resultCode` trên query), nhưng trạng thái đơn hàng nên dựa vào IPN.

### 8. Cấu hình đề xuất

```
Momo:PartnerCode
Momo:AccessKey
Momo:SecretKey
Momo:Endpoint
Momo:RedirectUrl   // ví dụ: https://yourdomain.com/Payment/MomoReturn
Momo:IpnUrl        // ví dụ: https://yourdomain.com/Payment/MomoIpn
```

### 9. Tích hợp vào Checkout

- Khi user chọn “Chuyển khoản” → “MoMo”:
  1. Trong POST Checkout, gọi service tạo `payUrl`.
  2. `return Redirect(payUrl);`
- Tạo `PaymentController`:
  - `MomoIpn` (POST): verify signature, đối chiếu `amount`, update đơn hàng.
  - `MomoReturn` (GET): hiển thị kết quả cho user.

### 10. Bảo mật & kiểm thử

- Không log `secretKey`.
- Chỉ set `Paid` sau IPN hợp lệ và `amount` khớp.
- Dùng ngrok/public URL để nhận IPN khi dev cục bộ.
- Test đủ case: success, cancel, sai chữ ký, sai số tiền.

### 11. Checklist nhanh

- [ ] Có `partnerCode`, `accessKey`, `secretKey`, `endpoint`.
- [ ] Implement HMAC ký đúng thứ tự tham số.
- [ ] Gọi API create, nhận `payUrl`, redirect user.
- [ ] Verify IPN: chữ ký + số tiền + resultCode.
- [ ] Return page hiển thị kết quả, không quyết định trạng thái.
- [ ] Test sandbox, cả success/fail/hủy.
