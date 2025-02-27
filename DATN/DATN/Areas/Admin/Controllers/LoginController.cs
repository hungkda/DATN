using Microsoft.AspNetCore.Mvc;
using DATN.Areas.Admin.Models;
using DATN.Models;
using System.Text;
using NuGet.Protocol;
using System.Security.Cryptography;

namespace DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly DATNDbContext _context;
        public LoginController(DATNDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); //trả về trạng thái lỗi
            }

            //xử lý logic đăng nhập tại đây
            var pass = model.Password;
            var dataLogin = (from userstaff in _context.UserStaffs
                             join staff in _context.Staff on userstaff.Staff equals staff.Id
                             join position in _context.Positions on staff.Position equals position.Id
                             where userstaff.Username.Equals(model.Email)
                                && userstaff.Password.Equals(pass)
                                && position.Name.Equals("Phó Khoa")
                             select userstaff).FirstOrDefault();

            var data = dataLogin.ToJson();
            if (dataLogin != null)
            {
                //lưu session khi đăng nhập thành công
                HttpContext.Session.SetString("AdminLogin", data);

                return RedirectToAction("Index", "Dashboard");
            }
            TempData["ErrorMessage"] = "Bạn không có quyền hạn truy cập mục này";
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLogin");
            return RedirectToAction("Index");
        }

        static string GetSHA26Hash(string input)
        {
            string hash = "";
            using (var sha256 = new SHA256Managed())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
            return hash;
        }
    }
}
