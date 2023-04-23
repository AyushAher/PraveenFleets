using System.ComponentModel.DataAnnotations;
using Enums.Account;
using Enums.Employee;
using Shared.Requests.Common;

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
        public string Email { get; set; } = string.Empty;

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

        public Guid ParentEntityId { get; set; }
        
        public Guid RoleId { get; set; }

        public string Role { get; set; } = string.Empty;

        public UserType UserType { get; set; }

        public Gender Gender { get; set; }
        
        public Salutation Salutation { get; set; }
        
        public List<WeekDays> WeeklyOffs { get; set; }

        public AddressRequest Address { get; set; }
    }
}
