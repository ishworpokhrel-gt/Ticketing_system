using HEI.Support.Common.Models;
using HEI.Support.Common.Models.Enum;
using HEI.Support.Domain.Entities;
using HEI.Support.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HEI.Support.WebApp.Controllers
{
	public class TicketController : Controller
	{
        private readonly ITicketService _ticketService;
        private readonly UserManager<ApplicationUser> _userManager;
        public TicketController(ITicketService ticketService, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _ticketService = ticketService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
		{
            var tickets =  await _ticketService.GetAllTicketsAsync();
            string roleName = "Support";
            var users = await _ticketService.GetUsersByRoleAsync(roleName);
            var userSelectList = users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.UserName
            }).ToList();

            ViewBag.Users = userSelectList;
            return View(tickets);
		}

        [HttpGet]
        public async Task<IActionResult> Create()
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
        public async Task<IActionResult> Create(TicketViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.GetUserAsync(User);
                await _ticketService.CreateTicketAsync(model, user);
                return RedirectToAction(nameof(Index));
            }
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
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignTo(TicketLogViewModel model, ApplicationUser user)
        {
            if (model.TicketId == Guid.Empty || string.IsNullOrEmpty(model.UserId))
            {
                return BadRequest("Invalid ticket or user.");
            }

            await _ticketService.AssignTicketAsync(model.TicketId, model.UserId, user);

            return RedirectToAction("Index");
        }


    }
}
