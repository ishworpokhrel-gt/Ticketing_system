using Microsoft.AspNetCore.Identity;

namespace HEI.Support.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? LastLogoutTime { get; set; }
        public int AccessFailedCount { get; set; }
        public string? TelNumber { get; set; }
    }
}
