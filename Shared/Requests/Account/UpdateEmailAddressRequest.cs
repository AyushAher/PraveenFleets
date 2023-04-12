using System.ComponentModel.DataAnnotations;

namespace Shared.Requests.Account;
public class UpdateEmailRequest
{
    [Required]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "The Email is not a valid e-mail address")]
    [StringLength(256, ErrorMessage = "The {0} must be at max {1} characters long.")]
    public string OldEmailAddress { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "The Email is not a valid e-mail address")]
    [StringLength(256, ErrorMessage = "The {0} must be at max {1} characters long.")]
    public string NewEmailAddress { get; set; } = string.Empty;

    [StringLength(10)]
    public string VerificationCode { get; set; } = string.Empty;
}