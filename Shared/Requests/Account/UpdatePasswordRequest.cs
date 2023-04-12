using System.ComponentModel.DataAnnotations;

namespace Shared.Requests.Account
{
    public class UpdatePasswordRequest
    {
        [Required] public Guid UserId { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 6,
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string NewPasswordConfirm { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;
    }
}
