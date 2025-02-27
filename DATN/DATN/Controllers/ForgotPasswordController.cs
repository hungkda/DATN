using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DATN.Models;
using DATN.Services;
using DATN.ViewModels;
using NETCore.MailKit.Core;
using Org.BouncyCastle.Ocsp;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DATN.Controllers
{
    public class ForgotPassword : Controller
    {
        private readonly DATNDbContext _context;
        private readonly IEmailServices _emailService;

        public ForgotPassword(DATNDbContext context, IEmailServices emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }
        /*[HttpPost]
        public IActionResult Index(ViewModels.ForgotPasswordViewModels forgot)
        {
            var message = new Message(new string[] { "mhung4011@gmail.com" }, "Test", "<h1>Thanhf cong</h1>");
            _emailService.SendEmail(message);
            return RedirectToAction("Index");
        }*/

        [HttpPost]
        public async Task<IActionResult> Index(Email forgot)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.UserStaffs.FirstOrDefaultAsync(c => c.Username == forgot.EmailAddress);
                if (user != null)
                {
                    // Tạo token reset password
                    var newToken = Guid.NewGuid().ToString();

                    // Lưu token vào TempData với khóa duy nhất để sử dụng trong action ResetPassword
                    HttpContext.Session.SetString("Token", newToken);
                    HttpContext.Session.SetString("Email", forgot.EmailAddress);
                    // Gửi email với link reset password
                    var callbackUrl = Url.Action("ResetPassword", "ForgotPassword", new { token = newToken }, protocol: HttpContext.Request.Scheme);
                    var message = new Message(new string[] { forgot.EmailAddress }, "Khôi phục mật khẩu", $"Vui lòng khôi phục mật khẩu bằng cách <a href='{callbackUrl}'>ấn vào đây</a>.");
                    _emailService.SendEmail(message);

                    // Chuyển hướng người dùng đến trang thông báo thành công
                    return RedirectToAction("PasswordRecoverySuccess");
                }
                else
                {
                    ModelState.AddModelError("Email", "Email not found.");
                }
            }
            return View(forgot);
        }

        public IActionResult ResetPassword(string token)
        {
            // Kiểm tra token từ TempData
            var resetPasswordToken = HttpContext.Session.GetString("Token");
            var EmailReset = HttpContext.Session.GetString("Email");
            if (resetPasswordToken != token)
            {
                // Token không hợp lệ
                return RedirectToAction("PasswordResetError");
            }

            // Nếu token hợp lệ, hiển thị form reset mật khẩu
            var model = new ForgotPasswordViewModels { Token = token, Email = EmailReset };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ForgotPasswordViewModels model, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra token từ TempData
                var resetPasswordToken = HttpContext.Session.GetString("Token");
                if (resetPasswordToken != model.Token)
                {
                    // Token không hợp lệ
                    return RedirectToAction("PasswordResetError");
                }

                // Thiết lập mật khẩu mới cho người dùng
                var user = await _context.UserStaffs.FirstOrDefaultAsync(u => u.Username == model.Email);
                if (user != null)
                {
                    user.Password = model.Password;
                    user.UpdateBy = model.Email;
                    user.UpdateDate = DateTime.Now;


                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    // Chuyển hướng người dùng đến trang thông báo mật khẩu đã được reset thành công
                    TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    return RedirectToAction("PasswordResetError");
                }
            }

            return View(model);
        }

        public IActionResult PasswordRecoverySuccess()
        {
            return View();
        }
    }
}