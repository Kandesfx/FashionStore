using System;
using System.Web.Mvc;
using FashionStore.Filters;
using FashionStore.Models.ViewModels;
using FashionStore.Services.Interfaces;

namespace FashionStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            if (Session["UserId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (_userService.Login(model.Username, model.Password))
                {
                    var user = _userService.GetByUsername(model.Username);
                    Session["UserId"] = user.Id;
                    Session["Username"] = user.Username;
                    Session["Role"] = user.Role.RoleName;
                    Session["FullName"] = user.FullName;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            if (Session["UserId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            // Kiểm tra ModelState validation (Required, StringLength, EmailAddress, Compare, etc.)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Đăng ký user mới - method này sẽ:
                // 1. Kiểm tra username và email đã tồn tại chưa
                // 2. Hash password
                // 3. Tạo user mới và lưu vào database
                _userService.Register(model);
                
                // Đăng ký thành công - chuyển hướng về trang đăng nhập
                return RedirectToAction("Login", "Account");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        [AuthorizeRole("User", "Admin")]
        public ActionResult UserProfile()
        {
            var userId = (int)Session["UserId"];
            var user = _userService.GetById(userId);
            
            var model = new UserProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                Email = user.Email
            };

            return View(model);
        }

        // POST: Account/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("User", "Admin")]
        public ActionResult UserProfile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = (int)Session["UserId"];
                _userService.UpdateProfile(userId, model);
                
                // Update session
                var user = _userService.GetById(userId);
                Session["FullName"] = user.FullName;

                ViewBag.SuccessMessage = "Cập nhật thông tin thành công";
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }
    }
}

