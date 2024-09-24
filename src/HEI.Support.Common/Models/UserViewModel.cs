namespace HEI.Support.Common.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTimeOffset? RegistrationDate { get; set; }
        public string Role { get; set; }
        public List<string>? Roles { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? LastLogoutTime { get; set; }
        public int FailedLoginAttempts { get; set; }
    }
}
