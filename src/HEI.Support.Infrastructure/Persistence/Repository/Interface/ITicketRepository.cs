﻿using HEI.Support.Common.Models;
using HEI.Support.Domain.Entities;

namespace HEI.Support.Infrastructure.Persistence.Repository.Interface
{
    public interface ITicketRepository : IBaseRepository<Ticket>
    {
        Task<List<TicketViewModel>> GetAllTicketsAsync(DateTime? fromDate, DateTime? toDate, int? statusId = null, int? issueTypeId = null);
        Task<TicketCountViewModel> GetAllTicketsCountAsync(string? userId = null, DateTime? ticketDatetime = null);
        Task<string> GetTicketAssignee(Guid ticketId, int status);
        Task<TicketViewModel> GetTicketByIdAsync(Guid id);
    }
}
