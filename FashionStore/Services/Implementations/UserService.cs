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

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
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

            // Check if username exists
            if (GetByUsername(model.Username) != null)
                throw new InvalidOperationException("Tên đăng nhập đã tồn tại");

            // Check if email exists
            if (GetByEmail(model.Email) != null)
                throw new InvalidOperationException("Email đã được sử dụng");

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
    }
}
