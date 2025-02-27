using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DATN.Areas.StudentArea.ViewModels;
using DATN.Models;
using Newtonsoft.Json;

namespace DATN.Areas.StudentArea.Controllers
{
    [Area("StudentArea")]
    public class ChangePasswordController : BaseController
    {
        private readonly DATNDbContext _context;

        public ChangePasswordController(DATNDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(long? id)
        {
            /*var userSttudent = await _context.UserStudents
                .Include(u => u.StudentNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);*/
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IFormCollection form, long? id, ChangePassword changePassword)
        {
            if (!ModelState.IsValid)
            {
                return View(changePassword);
            }

            var userStudentSession = HttpContext.Session.GetString("StudentLogin");
            if (string.IsNullOrEmpty(userStudentSession))
            {
                // Handle the case where the session is missing
                return RedirectToAction("Login", "Account");
            }

            var user_student = JsonConvert.DeserializeObject<UserStudent>(userStudentSession);

            if (form["CurrentPassword"].ToString() != user_student.Password)
            {
                TempData["ErrorMessage"] = "Mật khẩu cũ không đúng";
                return View();
            }

            var userStudent = await _context.UserStudents.FirstOrDefaultAsync(u => u.Id == id);
            userStudent.Password = form["NewPassword"].ToString();

            _context.Update(userStudent);
            await _context.SaveChangesAsync();

            // Cập nhật phiên làm việc sau khi thay đổi mật khẩu
            HttpContext.Session.SetString("StudentLogin", JsonConvert.SerializeObject(userStudent));

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
