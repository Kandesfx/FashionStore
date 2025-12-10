using FashionStore.Models.Entities;
using FashionStore.Models.ViewModels;

namespace FashionStore.Services.Interfaces
{
    public interface IUserService
    {
        User GetById(int id);
        User GetByUsername(string username);
        User GetByEmail(string email);
        void Register(RegisterViewModel model);
        bool Login(string username, string password);
        void UpdateProfile(int userId, UserProfileViewModel model);
        void ChangePassword(int userId, string oldPassword, string newPassword);
        bool ValidateUser(string username, string password);
        void Add(User user);
        System.Collections.Generic.IEnumerable<User> GetAll();
        string GenerateResetToken(string email);
        bool VerifyResetToken(string email, string token);
        void ResetPasswordByToken(string email, string token, string newPassword);
    }
}

