using Microsoft.AspNetCore.Mvc;

namespace HEI.Support.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
