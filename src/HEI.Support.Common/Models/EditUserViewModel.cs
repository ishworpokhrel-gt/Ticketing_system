namespace HEI.Support.Common.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string TelNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsLockedOut { get; set; }
        // Add additional properties as necessary
    }
}
