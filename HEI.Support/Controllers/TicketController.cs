using Microsoft.AspNetCore.Mvc;

namespace HEI.Support.Controllers
{
	public class TicketController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
