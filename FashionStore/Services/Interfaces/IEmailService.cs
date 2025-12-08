namespace FashionStore.Services.Interfaces
{
    public interface IEmailService
    {
        void SendPasswordResetCode(string toEmail, string resetCode);
        bool SendEmail(string toEmail, string subject, string body);
    }
}

