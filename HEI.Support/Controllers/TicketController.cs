using HEI.Support.Data.Entities;
using HEI.Support.Models;
using HEI.Support.Models.Enum;
using HEI.Support.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HEI.Support.Controllers
{
	public class TicketController : Controller
	{
        private readonly ITicketService _ticketService;
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        public async Task<IActionResult> Index()
		{
            var tickets =  await _ticketService.GetAllTicketsAsync();

            return View(tickets);
		}

        [HttpGet]
        public IActionResult Create()
        {
            var issueTypes = Enum.GetValues(typeof(IssueType))
                         .Cast<IssueType>()
                         .Select(e => new SelectListItem
                         {
                             Value = ((int)e).ToString(),
                             Text = e.ToString()
                         }).ToList();

            // Pass the issue types to the view via ViewBag
            ViewBag.IssueTypes = issueTypes;

            var priority = Enum.GetValues(typeof(Priority))
                        .Cast<Priority>()
                        .Select(e => new SelectListItem
                        {
                            Value = ((int)e).ToString(),
                            Text = e.ToString()
                        }).ToList();

            // Pass the issue types to the view via ViewBag
            ViewBag.Priority = priority;
            return View();
        }

        // POST: /Admin/Tickets/Create
        [HttpPost]
        public async Task<IActionResult> Create(TicketViewModel model, ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
              
                await _ticketService.CreateTicketAsync(model, user);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}
