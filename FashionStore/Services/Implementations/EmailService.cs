using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using FashionStore.Services.Interfaces;

namespace FashionStore.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _enableSsl;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService()
        {
            // Đọc cấu hình từ Web.config
            _smtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
            _smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"] ?? "";
            _smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"] ?? "";
            _enableSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpEnableSsl"] ?? "true");
            _fromEmail = ConfigurationManager.AppSettings["FromEmail"] ?? _smtpUsername;
            _fromName = ConfigurationManager.AppSettings["FromName"] ?? "Fashion Store";
        }

        public void SendPasswordResetCode(string toEmail, string resetCode)
        {
            string subject = "Mã Xác Nhận Khôi Phục Mật Khẩu - Fashion Store";
            string body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <div style='background: linear-gradient(135deg, #403B4A 0%, #5a5463 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
            <h1 style='margin: 0; font-size: 24px;'>Fashion Store</h1>
        </div>
        <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px;'>
            <h2 style='color: #403B4A; margin-top: 0;'>Khôi Phục Mật Khẩu</h2>
            <p>Xin chào,</p>
            <p>Bạn đã yêu cầu khôi phục mật khẩu cho tài khoản của bạn.</p>
            <p>Mã xác nhận của bạn là:</p>
            <div style='background: white; border: 2px solid #403B4A; border-radius: 8px; padding: 20px; text-align: center; margin: 20px 0;'>
                <h1 style='color: #403B4A; font-size: 36px; letter-spacing: 5px; margin: 0; font-family: monospace;'>{resetCode}</h1>
            </div>
            <p>Mã này có hiệu lực trong <strong>10 phút</strong>.</p>
            <p>Nếu bạn không yêu cầu khôi phục mật khẩu, vui lòng bỏ qua email này.</p>
            <hr style='border: none; border-top: 1px solid #ddd; margin: 30px 0;'>
            <p style='color: #666; font-size: 12px; margin: 0;'>Email này được gửi tự động, vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>";

            SendEmail(toEmail, subject, body);
        }

        public bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
                {
                    // Nếu chưa cấu hình SMTP, log lỗi và return false
                    System.Diagnostics.Debug.WriteLine("SMTP chưa được cấu hình. Vui lòng cấu hình trong Web.config");
                    return false;
                }

                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = _enableSsl;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_fromEmail, _fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    client.Send(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi gửi email: {ex.Message}");
                return false;
            }
        }
    }
}

