using System.ComponentModel.DataAnnotations;

namespace Shared.Requests.Account;

public class LoginRequest
{

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

}