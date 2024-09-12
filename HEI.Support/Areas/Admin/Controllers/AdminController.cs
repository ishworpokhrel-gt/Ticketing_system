using Microsoft.AspNetCore.Mvc;

namespace HEI.Support.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
