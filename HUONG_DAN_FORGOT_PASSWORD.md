# Hướng Dẫn Chức Năng Quên Mật Khẩu (Forgot Password)

## Tổng Quan

Chức năng Quên Mật Khẩu cho phép người dùng khôi phục mật khẩu của họ thông qua email. Hệ thống sẽ gửi mã OTP 6 số đến email đã đăng ký để xác thực trước khi cho phép đặt lại mật khẩu mới.

## Luồng Hoạt Động

### 1. Kích Hoạt Chức Năng

- Link "Quên mật khẩu" chỉ xuất hiện sau khi người dùng đăng nhập **sai ít nhất 1 lần**
- Link sẽ hiển thị ngay dưới thông báo lỗi đăng nhập

### 2. Quy Trình Khôi Phục Mật Khẩu

#### Bước 1: Nhập Email

- Người dùng nhập email đã đăng ký trong hệ thống
- Hệ thống kiểm tra email có tồn tại trong database
- Nếu email không tồn tại: Hiển thị lỗi "Email này chưa được đăng ký trong hệ thống"
- Nếu email hợp lệ: Hệ thống tạo mã OTP 6 số và gửi qua email

#### Bước 2: Xác Nhận Mã OTP

- Người dùng nhập mã OTP 6 số đã nhận được qua email
- Mã OTP có hiệu lực trong **10 phút**
- Nếu mã sai hoặc hết hạn: Hiển thị lỗi tương ứng
- Nếu mã đúng: Chuyển sang bước đặt lại mật khẩu

#### Bước 3: Đặt Lại Mật Khẩu

- Người dùng nhập mật khẩu mới (tối thiểu 6 ký tự)
- Xác nhận mật khẩu mới
- Sau khi đặt lại thành công: Chuyển về trang đăng nhập với thông báo thành công

## Cấu Hình Email (SMTP)

### Bước 1: Cấu Hình Web.config

Mở file `Web.config` và cập nhật các thông tin SMTP trong phần `<appSettings>`:

```xml
<appSettings>
  <!-- Email Configuration -->
  <add key="SmtpServer" value="smtp.gmail.com" />
  <add key="SmtpPort" value="587" />
  <add key="SmtpUsername" value="your-email@gmail.com" />
  <add key="SmtpPassword" value="your-app-password" />
  <add key="SmtpEnableSsl" value="true" />
  <add key="FromEmail" value="your-email@gmail.com" />
  <add key="FromName" value="Fashion Store" />
</appSettings>
```

### Bước 2: Cấu Hình Gmail (Nếu sử dụng Gmail)

1. **Bật xác thực 2 bước** cho tài khoản Gmail
2. **Tạo App Password**:
   - Vào [Google Account Settings](https://myaccount.google.com/)
   - Chọn "Security" → "2-Step Verification" → "App passwords"
   - Tạo mật khẩu ứng dụng mới
   - Sử dụng mật khẩu này cho `SmtpPassword`

### Bước 3: Cấu Hình Email Khác

Nếu sử dụng email provider khác, cập nhật các thông số tương ứng:

- **Outlook/Hotmail**: `smtp-mail.outlook.com`, port `587`
- **Yahoo**: `smtp.mail.yahoo.com`, port `587`
- **Custom SMTP**: Sử dụng thông tin SMTP của nhà cung cấp

## Cấu Trúc Code

### 1. EmailService

**File**: `Services/Implementations/EmailService.cs`

- Chịu trách nhiệm gửi email OTP
- Sử dụng SMTP để gửi email
- Email được format dạng HTML với thiết kế đẹp

**Phương thức chính**:

- `SendPasswordResetCode(string toEmail, string resetCode)`: Gửi mã OTP
- `SendEmail(string toEmail, string subject, string body)`: Gửi email tổng quát

### 2. UserService

**File**: `Services/Implementations/UserService.cs`

**Phương thức liên quan**:

- `GenerateResetToken(string email)`: Tạo mã OTP và gửi email
- `VerifyResetToken(string email, string token)`: Xác thực token
- `ResetPasswordByToken(string email, string token, string newPassword)`: Đặt lại mật khẩu

### 3. AccountController

**File**: `Controllers/AccountController.cs`

**Các Action**:

- `ForgotPassword()` (GET): Hiển thị form nhập email
- `ForgotPassword(ForgotPasswordViewModel)` (POST): Xử lý yêu cầu khôi phục
- `VerifyCode(string email)` (GET): Hiển thị form nhập mã OTP
- `VerifyCode(VerifyCodeViewModel)` (POST): Xác thực mã OTP
- `ResetPassword(string email, string token)` (GET): Hiển thị form đặt lại mật khẩu
- `ResetPassword(ResetPasswordViewModel)` (POST): Xử lý đặt lại mật khẩu
- `ResendCode(ResendCodeViewModel)` (POST): Gửi lại mã OTP

### 4. Views

- `Views/Account/ForgotPassword.cshtml`: Form nhập email
- `Views/Account/VerifyCode.cshtml`: Form nhập mã OTP
- `Views/Account/ResetPassword.cshtml`: Form đặt lại mật khẩu

## Bảo Mật

### 1. Session Management

- Mã OTP được lưu trong Session với thời gian hết hạn 10 phút
- Token reset password được tạo bằng GUID để đảm bảo tính duy nhất
- Tất cả session data được xóa sau khi đặt lại mật khẩu thành công

### 2. Validation

- Email phải tồn tại trong hệ thống trước khi gửi OTP
- Mã OTP phải đúng và chưa hết hạn
- Mật khẩu mới phải có tối thiểu 6 ký tự
- Xác nhận mật khẩu phải khớp với mật khẩu mới

### 3. Rate Limiting

- Mỗi mã OTP chỉ có hiệu lực 10 phút
- Có thể thêm giới hạn số lần gửi lại mã trong tương lai

## Xử Lý Lỗi

### Lỗi Thường Gặp

1. **Email không tồn tại**

   - Thông báo: "Email này chưa được đăng ký trong hệ thống. Vui lòng kiểm tra lại."
   - Giải pháp: Kiểm tra email đã đăng ký chưa

2. **Mã OTP sai**

   - Thông báo: "Mã xác nhận không đúng. Vui lòng thử lại."
   - Giải pháp: Kiểm tra lại mã trong email

3. **Mã OTP hết hạn**

   - Thông báo: "Mã xác nhận đã hết hạn. Vui lòng thử lại."
   - Giải pháp: Yêu cầu gửi lại mã mới

4. **Lỗi gửi email**
   - Thông báo: "Đã xảy ra lỗi khi gửi email. Vui lòng thử lại sau."
   - Giải pháp: Kiểm tra cấu hình SMTP trong Web.config

## Testing

### Test Case 1: Quên Mật Khẩu Thành Công

1. Đăng nhập sai 1 lần
2. Click "Quên mật khẩu"
3. Nhập email hợp lệ
4. Kiểm tra email nhận được mã OTP
5. Nhập mã OTP đúng
6. Đặt lại mật khẩu mới
7. Đăng nhập với mật khẩu mới

### Test Case 2: Email Không Tồn Tại

1. Nhập email chưa đăng ký
2. Kiểm tra thông báo lỗi hiển thị đúng

### Test Case 3: Mã OTP Sai

1. Nhập mã OTP sai
2. Kiểm tra thông báo lỗi

### Test Case 4: Mã OTP Hết Hạn

1. Đợi quá 10 phút sau khi nhận mã
2. Nhập mã OTP
3. Kiểm tra thông báo hết hạn

## Troubleshooting

### Email Không Được Gửi

1. **Kiểm tra cấu hình SMTP**:

   - Xác nhận `SmtpServer`, `SmtpPort`, `SmtpUsername`, `SmtpPassword` đúng
   - Với Gmail: Sử dụng App Password, không dùng mật khẩu thường

2. **Kiểm tra Firewall/Network**:

   - Đảm bảo port 587 không bị chặn
   - Kiểm tra kết nối internet

3. **Kiểm tra Logs**:
   - Xem Debug Output trong Visual Studio
   - Kiểm tra exception messages

### Link Quên Mật Khẩu Không Hiển Thị

- Đảm bảo đã đăng nhập sai ít nhất 1 lần
- Kiểm tra `ViewData.ModelState.IsValid == false` trong Login.cshtml

## Tùy Chỉnh

### Thay Đổi Thời Gian Hết Hạn OTP

Trong `AccountController.cs`, tìm dòng:

```csharp
Session["ResetCodeExpiry"] = DateTime.Now.AddMinutes(10);
```

Thay đổi số phút theo nhu cầu.

### Thay Đổi Format Email

Chỉnh sửa method `SendPasswordResetCode` trong `EmailService.cs` để thay đổi nội dung email.

## Lưu Ý Quan Trọng

1. **Production**: Đảm bảo cấu hình SMTP chính xác trước khi deploy
2. **Security**: Không bao giờ hiển thị mã OTP trên màn hình trong production
3. **Testing**: Luôn test với email thật trước khi deploy
4. **Backup**: Lưu trữ cấu hình SMTP an toàn

## Tài Liệu Tham Khảo

- [ASP.NET MVC Documentation](https://docs.microsoft.com/en-us/aspnet/mvc/)
- [System.Net.Mail Documentation](https://docs.microsoft.com/en-us/dotnet/api/system.net.mail)
- [Gmail App Passwords](https://support.google.com/accounts/answer/185833)
