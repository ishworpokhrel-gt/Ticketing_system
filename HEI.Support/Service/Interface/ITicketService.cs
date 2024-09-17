using HEI.Support.Models;

namespace HEI.Support.Service.Interface
{
    public interface ITicketService
    {
        Task CreateTicketAsync(TicketViewModel model, string userId);
        Task<TicketViewModel> GetTicketByIdAsync(Guid ticketId);
        Task<IEnumerable<TicketViewModel>> GetTicketsBySupportUserIdAsync(string supportUserId);
        Task<IEnumerable<TicketViewModel>> GetTicketsByUserIdAsync(string userId);
        Task UpdateTicketAsync(TicketViewModel model);
    }
}