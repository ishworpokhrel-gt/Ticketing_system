﻿using HEI.Support.Common.Models;
using HEI.Support.Common.Models.Enum;
using HEI.Support.Domain.Entities;
using HEI.Support.Infrastructure.Persistence;
using HEI.Support.Infrastructure.Persistence.Repository.Interface;
using HEI.Support.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace HEI.Support.Service.Implementation
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IAttachmentFileRepository _attachmentFileRepository;
        private readonly IActivityLogRepository _activityLogRepository;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWorkRepository _unitOfWork;


        public TicketService(ITicketRepository ticketRepository, IAttachmentFileRepository attachmentFileRepository,
            IActivityLogRepository activityLogRepository, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IUnitOfWorkRepository unitOfWork)
        {
            _ticketRepository = ticketRepository;
            _attachmentFileRepository = attachmentFileRepository;
            _activityLogRepository = activityLogRepository;
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }


        public async Task CreateTicketAsync(TicketViewModel model, ApplicationUser user)
        {
            _unitOfWork.BeginTransaction();

            try
            {
                // Create the ticket
                var ticket = new Ticket
                {
                    IssueTypeId = model.IssueType,
                    Description = model.Description,
                    Priority = model.Priority,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = user.Id
                };
                await _ticketRepository.AddAsync(ticket);

                // Create the activity log
                var activityLog = new ActivityLog
                {
                    TicketId = ticket.Id,
                    UserId = user.Id,
                    Status = ticket.Status,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = user.Id
                };
                await _activityLogRepository.AddAsync(activityLog);

                // Handle file attachments
                if (model.Attachment is not null)
                {
                    var attachedFiles = await UploadImageAsync(model.Attachment);



                    var userUploadItems = attachedFiles.Select(uploadedFileName => new AttachmentFile()
                    {
                        TicketID = ticket.Id,
                        FileUrl = uploadedFileName,
                        FileType = "image",
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = user.Id
                    }).ToList();
                    await _attachmentFileRepository.AddMultipleEntity(userUploadItems);
                }
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
        public async Task<List<TicketViewModel>> GetAllTicketsAsync(DateTime? fromDate, DateTime? toDate, int? statusId = null, int? issueTypeId = null)
        {
            var data = await _ticketRepository.GetAllTicketsAsync(fromDate, toDate, statusId, issueTypeId);
            return data;
        }
        public async Task<TicketCountViewModel> GetAllTicketsCountAsync(string? userId = null, DateTime? ticketDatetime = null)
        {
            var data = await _ticketRepository.GetAllTicketsCountAsync(userId, ticketDatetime);
            return data;
        }
        public async Task<List<string>> UploadImageAsync(List<IFormFile> files)
        {
            var uniqueFileName = new List<string>();
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Ticket");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            foreach (var item in files)
            {
                var renamedFileName = $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}{DateTime.Now.Millisecond}";

                var fileName = $"{renamedFileName}{Path.GetExtension(item.FileName)}";

                await using var stream = new FileStream(Path.Combine(folderPath, fileName), FileMode.Create);

                await item.CopyToAsync(stream);

                uniqueFileName.Add("/" + Path.Combine("Ticket", fileName));

            }
            return uniqueFileName;
        }
        public async Task<TicketViewModel> GetTicketByIdAsync(Guid ticketId)

        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with Id {ticketId} not found.");
            }
            return ticket;

        }
        public async Task<List<UserViewModel>> GetUsersByRoleAsync(string roleName)
        {
            // Check if the role exists
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role '{roleName}' not found.");
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            var result = new List<UserViewModel>();
            foreach (var user in usersInRole)
            {
                var userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    IsLockedOut = await _userManager.IsLockedOutAsync(user),
                    RegistrationDate = user.LockoutEnd,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                    FailedLoginAttempts = user.AccessFailedCount,
                    LastLoginTime = user.LastLoginTime,
                    LastLogoutTime = user.LastLogoutTime
                };
                result.Add(userViewModel);
            }

            return result;
        }
        public async Task AssignTicketAsync(Guid ticketId, string assigneeId, ApplicationUser user)
        {
            var ticket = await _ticketRepository.GetAsync(ticketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException("Ticket not found.");
            }
            ticket.Status = (int)TicketStatus.InProgress;
            await _ticketRepository.UpdateAsync(ticket);
            var assignee = new ActivityLog
            {
                TicketId = ticketId,
                UserId = assigneeId,
                Status = (int)TicketStatus.InProgress,
                CreatedBy = user.Id,
                CreatedDate = DateTime.UtcNow
            };
            await _activityLogRepository.AddAsync(assignee);
        }
        public async Task<bool> GetTicketStatus(Guid ticketId, int ticketStatusId)
        {
            _unitOfWork.BeginTransaction();

            try
            {
                var ticket = await _ticketRepository.GetAsync(ticketId);

                if (ticket != null && ticket.Status != ticketStatusId)
                {
                    ticket.Status = ticketStatusId;
                    await _ticketRepository.UpdateAsync(ticket);
                }
                var activity = await _activityLogRepository.GetActivityLogByTicketIdAndStatus(ticketId, ticketStatusId);

                if (activity != null && activity.Status != ticketStatusId)
                {
                    activity.Status = ticketStatusId;
                    await _activityLogRepository.UpdateAsync(activity);
                }
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
        public async Task<bool> UpdateTaskStatusAsync(Guid ticketId, string userId, int status)
        {
            _unitOfWork.BeginTransaction();
            var ticket = await _ticketRepository.GetAsync(ticketId);
            if (ticket != null && ticket.Status != status)
            {
                ticket.Status = status;
                await _ticketRepository.UpdateAsync(ticket);
                var activityLog = new ActivityLog
                {
                    TicketId = ticket.Id,
                    UserId = userId,
                    Status = status,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = userId
                };

                await _activityLogRepository.AddAsync(activityLog);

                _unitOfWork.Commit();
                return true;
            }
            return false;
        }
        public Task<IEnumerable<TicketViewModel>> GetTicketsBySupportUserIdAsync(string supportUserId)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<TicketViewModel>> GetTicketsByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }
        public Task UpdateTicketAsync(TicketViewModel model)
        {
            throw new NotImplementedException();
        }



    }
}