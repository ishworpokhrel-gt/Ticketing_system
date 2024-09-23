using System.ComponentModel.DataAnnotations;

namespace HEI.Support.Common.Models
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
