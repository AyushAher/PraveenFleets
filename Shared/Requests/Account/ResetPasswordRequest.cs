using System.ComponentModel.DataAnnotations;

namespace Shared.Requests.Account
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string EMail { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "The password must have lower and upper case alphabets with atleast one number and a special character.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
