using HEI.Support.Data.Entities;
using HEI.Support.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HEI.Support.Controllers
{
	public class EndUserController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public EndUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public IActionResult CreateTicket()
		{
			return View();
		}

		//[HttpPost]
		//public async Task<IActionResult> CreateTicket(Ticket model)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		return View(model);
		//	}

		//	//var user = await _userManager.GetUserAsync(User);
		//	model.CreatedDate = DateTime.UtcNow;
		//	model.Status = "Open";

		//	_context.Tickets.Add(model);
		//	await _context.SaveChangesAsync();

		//	return RedirectToAction("Index", "Home"); // Redirect to a confirmation or home page
		//}
	}
}
