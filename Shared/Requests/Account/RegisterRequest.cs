using System.ComponentModel.DataAnnotations;
using Enums.Account;

namespace Shared.Requests.Account
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Your First or Given name is required!")]
        [StringLength(60, MinimumLength = 1,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Your Last or Sur name is required!")]
        [StringLength(60, MinimumLength = 1,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Your primary email address is required!")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string EMail { get; set; } = string.Empty;

        [StringLength(25, MinimumLength = 6,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public bool EmailVerified { get; set; }

        public PrimaryRoles UserRole { get; set; } = PrimaryRoles.AdminRole;
    }
}
