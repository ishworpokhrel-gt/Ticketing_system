namespace HEI.Support.Areas.Admin.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsLockedOut { get; set; }
        // Add additional properties as necessary
    }
}
