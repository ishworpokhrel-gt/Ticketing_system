using HEI.Support.Data.Entities;
using HEI.Support.Models;

namespace HEI.Support.Service.Interface
{
    public interface ITicketService
    {
        Task CreateTicketAsync(TicketViewModel model, ApplicationUser user);
        Task<List<TicketViewModel>> GetAllTicketsAsync();
        Task<TicketViewModel> GetTicketByIdAsync(Guid ticketId);
        Task<IEnumerable<TicketViewModel>> GetTicketsBySupportUserIdAsync(string supportUserId);
        Task<IEnumerable<TicketViewModel>> GetTicketsByUserIdAsync(string userId);
        Task UpdateTicketAsync(TicketViewModel model);
    }
}