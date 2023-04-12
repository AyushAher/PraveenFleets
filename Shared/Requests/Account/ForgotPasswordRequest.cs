using System.ComponentModel.DataAnnotations;

namespace Shared.Requests.Account
{

    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "The primary email address used to login is required!")]
        [EmailAddress]
        [MaxLength(100)]
        public string EMail { get; set; } = string.Empty;
    }
}
