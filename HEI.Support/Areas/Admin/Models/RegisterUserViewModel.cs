using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HEI.Support.Areas.Admin.Models
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

        // Property to hold selected roles
        public List<string> SelectedRoles { get; set; } = new List<string>();

        // Property to hold available roles for the dropdown
        public List<SelectListItem> AvailableRoles { get; set; } = new List<SelectListItem>();
    }
}
