using System.ComponentModel.DataAnnotations;

namespace Shared.Requests.Account
{
    public class UpdateProfileRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Your First or Given name is required!")]
        [StringLength(60, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Your Last or Sur name is required!")]
        [StringLength(60, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Your primary email address is required!")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EMail { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
