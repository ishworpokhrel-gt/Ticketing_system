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
        public async Task<IActionResult> GetAllTicketsCountAsync(DateTime? start = null, DateTime? end = null)
        {
            //string? userId = null;
			var data = await _ticketService.GetAllTicketsCountAsync(null, start);
            return Ok(data);
        }
    }
}
