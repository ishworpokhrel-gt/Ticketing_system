using HEI.Support.Common.Models;
using HEI.Support.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace HEI.Support.Service.Interface
{
    public interface ITicketService
    {
        Task<bool> UpdateTaskStatusAsync(Guid ticketId, string userId, int status);
        Task AssignTicketAsync(Guid ticketId, string assigneeId, ApplicationUser user);
        Task CreateTicketAsync(TicketViewModel model, ApplicationUser user);
        Task<List<TicketViewModel>> GetAllTicketsAsync(int? statusId = null, int? issueTypeId = null);
        Task<TicketViewModel> GetTicketByIdAsync(Guid ticketId);
        Task<IEnumerable<TicketViewModel>> GetTicketsBySupportUserIdAsync(string supportUserId);
        Task<IEnumerable<TicketViewModel>> GetTicketsByUserIdAsync(string userId);
        Task<bool> GetTicketStatus(Guid ticketId, int ticketStatusId);
        Task<List<UserViewModel>> GetUsersByRoleAsync(string roleName);
        Task UpdateTicketAsync(TicketViewModel model);
        Task<List<string>> UploadImageAsync(List<IFormFile> files);
        Task<TicketCountViewModel> GetAllTicketsCountAsync(string? userId = null, DateTime? ticketDatetime = null);
    }
}