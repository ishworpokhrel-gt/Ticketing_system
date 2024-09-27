using HEI.Support.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HEI.Support.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardApiController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public DashboardApiController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        [HttpGet]
        [Route("GetAllTicketsCountAsync")]
        public async Task<IActionResult> GetAllTicketsCountAsync(string? userId = null, DateTime? ticketDatetime = null)
        {
            var data = await _ticketService.GetAllTicketsCountAsync(userId, ticketDatetime);
            return Ok(data);
        }
    }
}
