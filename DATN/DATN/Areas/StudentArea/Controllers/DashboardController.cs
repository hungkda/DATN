using Microsoft.AspNetCore.Mvc;

namespace DATN.Areas.StudentArea.Controllers
{
    public class DashboardController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
