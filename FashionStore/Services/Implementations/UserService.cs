using FashionStore.Models.Entities;
using FashionStore.Models.ViewModels;
using FashionStore.Repositories.Interfaces;
using FashionStore.Services.Interfaces;
using FashionStore.Utilities.Security;
using System;
using System.Collections.Generic;

namespace FashionStore.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public User GetById(int id)
        {
            return _userRepository.GetById(id);
        }

        public User GetByUsername(string username)
        {
            return _userRepository.GetByUsername(username);
        }

        public User GetByEmail(string email)
        {
            return _userRepository.GetByEmail(email);
        }

        public IEnumerable<User> GetAll()
        {
            return _userRepository.GetAll();
        }

        public void Register(RegisterViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Check if username exists - Tên tài khoản phải là duy nhất
            if (GetByUsername(model.Username) != null)
                throw new InvalidOperationException("Tên đăng nhập này đã tồn tại trong hệ thống. Vui lòng chọn tên khác.");

            // Check if email exists - Email phải là duy nhất, mỗi email chỉ được đăng ký 1 tài khoản
            if (GetByEmail(model.Email) != null)
                throw new InvalidOperationException("Email này đã được sử dụng để đăng ký tài khoản. Vui lòng sử dụng email khác hoặc đăng nhập nếu đã có tài khoản.");

            // Get User role (RoleId = 2)
            var userRole = _roleRepository.SingleOrDefault(r => r.RoleName == "User");
            if (userRole == null)
                throw new InvalidOperationException("Không tìm thấy role User");

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = PasswordHasher.HashPassword(model.Password),
                FullName = model.FullName,
                Phone = model.Phone,
                Address = model.Address,
                RoleId = userRole.Id,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            _userRepository.Add(user);
            _unitOfWork.Complete();
        }

        public bool Login(string username, string password)
        {
            var user = GetByUsername(username);
            if (user == null || !user.IsActive)
                return false;

            return PasswordHasher.VerifyPassword(password, user.PasswordHash);
        }

        public void UpdateProfile(int userId, UserProfileViewModel model)
        {
            var user = GetById(userId);
            if (user == null)
                throw new InvalidOperationException("Người dùng không tồn tại");

            user.FullName = model.FullName;
            user.Phone = model.Phone;
            user.Address = model.Address;
            user.Email = model.Email;

            _userRepository.Update(user);
            _unitOfWork.Complete();
        }

        public void ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = GetById(userId);
            if (user == null)
                throw new InvalidOperationException("Người dùng không tồn tại");

            if (!PasswordHasher.VerifyPassword(oldPassword, user.PasswordHash))
                throw new InvalidOperationException("Mật khẩu cũ không đúng");

            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            _userRepository.Update(user);
            _unitOfWork.Complete();
        }

        public bool ValidateUser(string username, string password)
        {
            return Login(username, password);
        }

        public void Add(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (user.Id == 0)
            {
                _userRepository.Add(user);
            }
            else
            {
                _userRepository.Update(user);
            }
            _unitOfWork.Complete();
        }

        public string GenerateResetToken(string email)
        {
            var user = GetByEmail(email);
            if (user == null)
                throw new InvalidOperationException("Email không tồn tại trong hệ thống");

            // Generate 6-digit code
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();
            
            // Gửi email chứa mã OTP
            try
            {
                _emailService.SendPasswordResetCode(email, code);
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng vẫn trả về code để không làm gián đoạn flow
                System.Diagnostics.Debug.WriteLine($"Không thể gửi email: {ex.Message}");
            }
            
            return code;
        }

        public bool VerifyResetToken(string email, string token)
        {
            // Token verification will be handled in controller using Session
            // This method is kept for consistency
            var user = GetByEmail(email);
            return user != null;
        }

        public void ResetPasswordByToken(string email, string token, string newPassword)
        {
            var user = GetByEmail(email);
            if (user == null)
                throw new InvalidOperationException("Email không tồn tại trong hệ thống");

            // Token verification should be done in controller before calling this
            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            _userRepository.Update(user);
            _unitOfWork.Complete();
        }
    }
}
