using System;
using System.Linq;
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
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Người dùng không tồn tại");
                        return View(model);
                    }

                    Session["UserId"] = user.Id;
                    Session["Username"] = user.Username;
                    Session["Role"] = user.Role?.RoleName ?? "User";
                    Session["FullName"] = user.FullName;
                    
                    // Reset số lần đăng nhập sai khi đăng nhập thành công
                    Session.Remove("LoginFailedAttempts");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Tăng số lần đăng nhập sai
                    int failedAttempts = Session["LoginFailedAttempts"] != null ? (int)Session["LoginFailedAttempts"] : 0;
                    Session["LoginFailedAttempts"] = failedAttempts + 1;
                    
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
            // Thêm tất cả lỗi validation vào summary (bao gồm lỗi Compare Password)
            bool hasValidationErrors = !ModelState.IsValid;
            
            if (hasValidationErrors)
            {
                // Thu thập tất cả lỗi property-level trước (tránh modify collection trong khi iterate)
                var errorsToAdd = new System.Collections.Generic.List<string>();
                foreach (var key in ModelState.Keys.ToList()) // ToList() để tạo bản sao
                {
                    if (!string.IsNullOrEmpty(key)) // Bỏ qua key rỗng (đã là summary)
                    {
                        var errors = ModelState[key].Errors;
                        foreach (var error in errors)
                        {
                            // Thu thập lỗi để thêm vào summary sau
                            errorsToAdd.Add(error.ErrorMessage);
                        }
                    }
                }
                
                // Thêm tất cả lỗi vào summary (key rỗng) để hiển thị trong validation summary
                foreach (var errorMsg in errorsToAdd)
                {
                    ModelState.AddModelError("", errorMsg);
                }
            }

            // Validate username uniqueness - Tên tài khoản phải là duy nhất
            bool hasErrors = hasValidationErrors;
            
            if (!string.IsNullOrWhiteSpace(model.Username))
            {
                var existingUserByUsername = _userService.GetByUsername(model.Username);
                if (existingUserByUsername != null)
                {
                    var errorMsg = "Tên đăng nhập này đã tồn tại trong hệ thống. Vui lòng chọn tên khác.";
                    ModelState.AddModelError("Username", errorMsg);
                    ModelState.AddModelError("", errorMsg); // Thêm vào summary
                    hasErrors = true;
                }
            }

            // Validate email uniqueness - Email phải là duy nhất, mỗi email chỉ được đăng ký 1 tài khoản
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var existingUserByEmail = _userService.GetByEmail(model.Email);
                if (existingUserByEmail != null)
                {
                    var errorMsg = "Email này đã được sử dụng để đăng ký tài khoản. Vui lòng sử dụng email khác hoặc đăng nhập nếu đã có tài khoản.";
                    ModelState.AddModelError("Email", errorMsg);
                    ModelState.AddModelError("", errorMsg); // Thêm vào summary
                    hasErrors = true;
                }
            }

            // Nếu có lỗi (validation hoặc uniqueness), return để hiển thị tất cả lỗi cùng lúc
            if (hasErrors)
            {
                return View(model);
            }

            try
            {
                _userService.Register(model);
                
                // Auto login after registration
                var user = _userService.GetByUsername(model.Username);
                if (user != null)
                {
                    Session["UserId"] = user.Id;
                    Session["Username"] = user.Username;
                    Session["Role"] = user.Role?.RoleName ?? "User";
                    Session["FullName"] = user.FullName;
                }

                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                // Handle specific validation errors from UserService
                if (ex.Message.Contains("Tên đăng nhập"))
                {
                    ModelState.AddModelError("Username", ex.Message);
                    ModelState.AddModelError("", ex.Message); // Thêm vào summary
                }
                else if (ex.Message.Contains("Email"))
                {
                    ModelState.AddModelError("Email", ex.Message);
                    ModelState.AddModelError("", ex.Message); // Thêm vào summary
                }
                else
                {
                    ModelState.AddModelError("", ex.Message);
                }
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

        // GET: Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            if (Session["UserId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kiểm tra email có tồn tại trong hệ thống không
                var user = _userService.GetByEmail(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Email này chưa được đăng ký trong hệ thống. Vui lòng kiểm tra lại.");
                    return View(model);
                }

                // Generate reset token và gửi email
                var resetCode = _userService.GenerateResetToken(model.Email);
                
                // Store in session for verification
                Session["ResetCode"] = resetCode;
                Session["ResetEmail"] = model.Email;
                Session["ResetCodeExpiry"] = DateTime.Now.AddMinutes(10); // 10 minutes expiry

                // Không hiển thị mã OTP trên màn hình (đã gửi qua email)
                TempData["SuccessMessage"] = "Mã xác nhận đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư.";

                return RedirectToAction("VerifyCode", new { email = model.Email });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi gửi email. Vui lòng thử lại sau.");
            }

            return View(model);
        }

        // GET: Account/VerifyCode
        public ActionResult VerifyCode(string email)
        {
            if (Session["ResetCode"] == null || Session["ResetEmail"] == null)
            {
                return RedirectToAction("ForgotPassword");
            }

            if (Session["ResetEmail"].ToString() != email)
            {
                return RedirectToAction("ForgotPassword");
            }

            var model = new VerifyCodeViewModel
            {
                Email = email
            };

            return View(model);
        }

        // POST: Account/VerifyCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if code exists in session
            if (Session["ResetCode"] == null || Session["ResetEmail"] == null)
            {
                ModelState.AddModelError("", "Mã xác nhận đã hết hạn. Vui lòng thử lại.");
                return View(model);
            }

            // Check expiry
            if (Session["ResetCodeExpiry"] != null && (DateTime)Session["ResetCodeExpiry"] < DateTime.Now)
            {
                Session.Remove("ResetCode");
                Session.Remove("ResetEmail");
                Session.Remove("ResetCodeExpiry");
                ModelState.AddModelError("", "Mã xác nhận đã hết hạn. Vui lòng thử lại.");
                return View(model);
            }

            // Verify code
            var storedCode = Session["ResetCode"].ToString();
            var storedEmail = Session["ResetEmail"].ToString();

            if (storedCode != model.Code || storedEmail != model.Email)
            {
                ModelState.AddModelError("", "Mã xác nhận không đúng. Vui lòng thử lại.");
                return View(model);
            }

            // Code verified - create token and redirect to reset password
            var token = Guid.NewGuid().ToString();
            Session["ResetToken"] = token;
            Session["ResetTokenEmail"] = model.Email;

            return RedirectToAction("ResetPassword", new { email = model.Email, token = token });
        }

        // GET: Account/ResetPassword
        public ActionResult ResetPassword(string email, string token)
        {
            if (Session["ResetToken"] == null || Session["ResetTokenEmail"] == null)
            {
                return RedirectToAction("ForgotPassword");
            }

            if (Session["ResetToken"].ToString() != token || Session["ResetTokenEmail"].ToString() != email)
            {
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        // POST: Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Verify token
            if (Session["ResetToken"] == null || Session["ResetTokenEmail"] == null)
            {
                ModelState.AddModelError("", "Phiên làm việc đã hết hạn. Vui lòng thử lại.");
                return View(model);
            }

            if (Session["ResetToken"].ToString() != model.Token || Session["ResetTokenEmail"].ToString() != model.Email)
            {
                ModelState.AddModelError("", "Token không hợp lệ. Vui lòng thử lại.");
                return View(model);
            }

            try
            {
                // Reset password
                _userService.ResetPasswordByToken(model.Email, model.Token, model.NewPassword);

                // Clear session
                Session.Remove("ResetCode");
                Session.Remove("ResetEmail");
                Session.Remove("ResetCodeExpiry");
                Session.Remove("ResetToken");
                Session.Remove("ResetTokenEmail");

                TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập với mật khẩu mới.";
                return RedirectToAction("Login");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi. Vui lòng thử lại sau.");
            }

            return View(model);
        }

        // POST: Account/ResendCode
        [HttpPost]
        public JsonResult ResendCode(ResendCodeViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model?.Email))
                {
                    return Json(new { success = false, message = "Email không được để trống" });
                }

                // Kiểm tra email có tồn tại không
                var user = _userService.GetByEmail(model.Email);
                if (user == null)
                {
                    return Json(new { success = false, message = "Email này chưa được đăng ký trong hệ thống." });
                }

                var resetCode = _userService.GenerateResetToken(model.Email);
                
                Session["ResetCode"] = resetCode;
                Session["ResetEmail"] = model.Email;
                Session["ResetCodeExpiry"] = DateTime.Now.AddMinutes(10);

                return Json(new { success = true, message = "Mã xác nhận mới đã được gửi đến email của bạn!" });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

