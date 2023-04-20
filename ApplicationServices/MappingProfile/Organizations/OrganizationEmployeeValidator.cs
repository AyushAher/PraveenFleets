using Domain.Organization;
using FluentValidation;

namespace ApplicationServices.MappingProfile.Organizations;

public class OrganizationEmployeeValidator : AbstractValidator<OrganizationEmployee>
{
    public OrganizationEmployeeValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .NotNull()
            .EmailAddress();

        RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Gender)
            .NotNull()
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.Salutation)
            .NotNull()
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.WeeklyOff)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.AddressId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotNull()
            .NotEmpty();

    }
}