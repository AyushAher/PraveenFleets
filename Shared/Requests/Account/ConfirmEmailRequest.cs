using System.ComponentModel.DataAnnotations;

namespace Shared.Requests.Account;

public class ConfirmEmailRequest
{
    [Required(ErrorMessage = "The primary email address being verified is mandatory")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    [MaxLength(100)]
    public string EMail { get; set; } = string.Empty;

    [Required(ErrorMessage = "The confirmation token of the email address being verified is required!")]
    [Display(Name = "Confirmation Token")]
    public string Token { get; set; } = string.Empty;
}

