using System.ComponentModel.DataAnnotations;

namespace HEI.Support.Areas.Admin.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.")]
        [RegularExpression(@"^(?=.*\d)(?=.*\W).+", ErrorMessage = "The password must contain at least one digit and one special character.")]
        public string Password { get; set; }

    }
}
