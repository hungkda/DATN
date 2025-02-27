using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DATN.Models;
using DATN.ViewModels;
using Newtonsoft.Json;

namespace DATN.Controllers
{
    public class ChangePasswordController : BaseController
    {
        private readonly DATNDbContext _context;

        public ChangePasswordController(DATNDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(long? id)
        {
            /*var userStaff = await _context.UserStaffs
                .Include(u => u.StaffNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            return View(userStaff);*/

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

            var userStaffSession = HttpContext.Session.GetString("StaffLogin");
            if (string.IsNullOrEmpty(userStaffSession))
            {
                // Handle the case where the session is missing
                return RedirectToAction("Login", "Account");
            }

            var user_staff = JsonConvert.DeserializeObject<UserStaff>(userStaffSession);

            if (form["CurrentPassword"].ToString() != user_staff.Password)
            {
                TempData["ErrorMessage"] = "Mật khẩu cũ không đúng";
                return View();
            }

            var userStaff = await _context.UserStaffs.FirstOrDefaultAsync(u => u.Id == id);
            userStaff.Password = form["NewPassword"].ToString();

            _context.Update(userStaff);
            await _context.SaveChangesAsync();

            // Cập nhật phiên làm việc sau khi thay đổi mật khẩu
            HttpContext.Session.SetString("StaffLogin", JsonConvert.SerializeObject(userStaff));

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công";
            return RedirectToAction("Index", "Attendance");
        }
    }
}
