using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HEI.Support.Common.Models
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Telephone Number is required")]
        public string TelNumber { get; set; }
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string SelectedRole { get; set; }
        public List<SelectListItem> AvailableRoles { get; set; } = new List<SelectListItem>();
    }
}
