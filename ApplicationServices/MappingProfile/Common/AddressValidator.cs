using Domain.Common;
using FluentValidation;

namespace ApplicationServices.MappingProfile.Common;

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(a => a.ParentId)
            .NotEmpty().WithMessage("ParentId is required.");

        RuleFor(a => a.Attention)
            .NotEmpty()
            .WithMessage("Attention is required.")
            .MaximumLength(100)
            .WithMessage("Attention must not exceed 100 characters.");

        RuleFor(a => a.ContactNumber)
            .NotEmpty()
            .WithMessage("ContactNumber is required.")
            .MaximumLength(15)
            .WithMessage("ContactNumber must not exceed 15 characters.");

        RuleFor(a => a.AddressLine1)
            .NotEmpty()
            .WithMessage("AddressLine1 is required.")
            .MaximumLength(200)
            .WithMessage("AddressLine1 must not exceed 200 characters.");

        RuleFor(a => a.AddressLine2)
            .MaximumLength(200)
            .WithMessage("AddressLine2 must not exceed 200 characters.");

        RuleFor(a => a.City)
            .NotEmpty()
            .WithMessage("City is required.")
            .MaximumLength(100)
            .WithMessage("City must not exceed 100 characters.");

        RuleFor(a => a.PinCode)
            .NotEmpty()
            .WithMessage("Pincode is required.")
            .MaximumLength(10)
            .WithMessage("Pincode must not exceed 10 characters.");

        RuleFor(a => a.State)
            .NotEmpty()
            .WithMessage("State is required.")
            .MaximumLength(100)
            .WithMessage("State must not exceed 100 characters.");

        RuleFor(a => a.Country)
            .NotEmpty()
            .WithMessage("Country is required.")
            .MaximumLength(100)
            .WithMessage("Country must not exceed 100 characters.");

    }
}