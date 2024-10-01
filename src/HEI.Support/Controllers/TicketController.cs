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
        public TicketController(ITicketService ticketService, UserManager<ApplicationUser> userManager)
        {
            _ticketService = ticketService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, int? status = null, int? issueTypeId = null)
        {
            var tickets = await _ticketService.GetAllTicketsAsync(fromDate, toDate, status, issueTypeId);
            string roleName = "Support";
            var users = await _ticketService.GetUsersByRoleAsync(roleName);
            var userSelectList = users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.UserName
            }).ToList();

            ViewBag.Users = userSelectList;

            var ticketStatus = Enum.GetValues(typeof(TicketStatus))
                         .Cast<TicketStatus>()
                         .Select(e => new SelectListItem
                         {
                             Value = ((int)e).ToString(),
                             Text = e.ToString()
                         }).ToList();
            ViewBag.TicketStatus = ticketStatus;
            var issueTypes = Enum.GetValues(typeof(IssueType))
                        .Cast<IssueType>()
                        .Select(e => new SelectListItem
                        {
                            Value = ((int)e).ToString(),
                            Text = e.ToString()
                        }).ToList();

            ViewBag.IssueTypes = issueTypes;
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

            ViewBag.IssueTypes = issueTypes;

            var priority = Enum.GetValues(typeof(Priority))
                        .Cast<Priority>()
                        .Select(e => new SelectListItem
                        {
                            Value = ((int)e).ToString(),
                            Text = e.ToString()
                        }).ToList();

            ViewBag.Priority = priority;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(TicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                await _ticketService.CreateTicketAsync(model, user);
                TempData["IsSuccess"] = true;
                TempData["Message"] = "Ticket created successfully.";
                return RedirectToAction("Index");
            }

            var issueTypes = Enum.GetValues(typeof(IssueType))
                         .Cast<IssueType>()
                         .Select(e => new SelectListItem
                         {
                             Value = ((int)e).ToString(),
                             Text = e.ToString()
                         }).ToList();

            ViewBag.IssueTypes = issueTypes;

            var priority = Enum.GetValues(typeof(Priority))
                        .Cast<Priority>()
                        .Select(e => new SelectListItem
                        {
                            Value = ((int)e).ToString(),
                            Text = e.ToString()
                        }).ToList();

            ViewBag.Priority = priority;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> GetStatus(TicketStatusViewModel model)
        {
            var result = await _ticketService.GetTicketStatus(model.TicketId, model.TicketStatusId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AssignTo(TicketLogViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (model.TicketId == Guid.Empty || string.IsNullOrEmpty(model.UserId))
            {
                return BadRequest("Invalid ticket or user.");
            }

            await _ticketService.AssignTicketAsync(model.TicketId, model.UserId, user);

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> PickTask(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            int status = (int)TicketStatus.InProgress;
            bool success = await _ticketService.UpdateTaskStatusAsync(id, user.Id, status);
            TempData["IsSuccess"] = success;
            TempData["Message"] = success ? "Task in progress !!" : "Task already picked !!" ;
           
            return RedirectToAction("Index");
        }
       
        [HttpGet]
        public async Task<IActionResult> CloseTask(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            int status = (int)TicketStatus.Closed;
            bool success = await _ticketService.UpdateTaskStatusAsync(id, user.Id, status);
            TempData["IsSuccess"] = success;
            TempData["Message"] = success ? "Task closed!!" : "Task already closed!!";
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Ticket");
        }
        [HttpGet]
        public async Task<IActionResult> TicketDetails(Guid id)
        {
            var ticketDetails = await _ticketService.GetTicketByIdAsync(id);
            if (ticketDetails == null)
            {
                return NotFound();
            }

            return View(ticketDetails);
        }
    }
}
