using FluentValidation;

namespace ApplicationServices.MappingProfile.Organizations;

public class OrganizationValidators :  AbstractValidator<Domain.Organization.Organizations>
{
    public OrganizationValidators()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200)
            .WithMessage("Maximum length should be 200 characters")
            .MinimumLength(1)
            .WithMessage("Minimum length should be 1 characters");

        RuleFor(x => x.GstNumber)
            .Length(15)
            .WithMessage("The length should be 15 characters");

        RuleFor(x => x.AddressId)
            .NotEmpty()
            .NotNull();

    }
}