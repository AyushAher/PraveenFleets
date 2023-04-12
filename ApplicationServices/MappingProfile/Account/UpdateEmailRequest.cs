using System.Linq.Expressions;
using FluentValidation;
using Shared.Requests.Account;

namespace ApplicationServices.MappingProfile.Account
{
    public class UpdateEmailRequestValidator : AbstractValidator<UpdateEmailRequest>
    {
        /// <summary>
        /// Validation rules for UpdateEmailRequestValidator Class
        /// </summary>
        public UpdateEmailRequestValidator()
        {
            RuleFor((Expression<Func<UpdateEmailRequest, string>>)(p => p.OldEmailAddress))
                .NotEmpty()
                .WithMessage("The Old Email field is a required field")
                .EmailAddress()
                .WithMessage("The Old Email is not a valid e-mail address.")
                .MaximumLength(256)
                .WithMessage(
                    "The EMail address must be at max 256 characters long.");

            RuleFor((Expression<Func<UpdateEmailRequest, string>>)(p => p.NewEmailAddress))
                .NotEmpty()
                .WithMessage("The New Email field is a required field")
                .EmailAddress()
                .WithMessage("The New Email is not a valid e-mail address.")
                .MaximumLength(256)
                .WithMessage("The New EMail address must be at max 256 characters long.")
                .NotEqual(
                    (Expression<Func<UpdateEmailRequest, string>>)(x => x.OldEmailAddress))
                .WithMessage(
                    "The current email address and new email address are same");

            RuleFor((Expression<Func<UpdateEmailRequest, string>>)(p => p.VerificationCode))
                .MaximumLength(10)
                .WithMessage(
                    "Verification code must be at max 10 characters long.");
        }
    }
}
