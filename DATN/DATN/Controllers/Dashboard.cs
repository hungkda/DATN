using Microsoft.AspNetCore.Mvc;

namespace DATN.Controllers
{
    public class Dashboard : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
