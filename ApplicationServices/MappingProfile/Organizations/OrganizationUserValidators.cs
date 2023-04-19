using Domain.Organization;
using FluentValidation;

namespace ApplicationServices.MappingProfile.Organizations;

public class OrganizationUserValidators:AbstractValidator<OrganizationUsers>
{
    public OrganizationUserValidators()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.UserId)
            .NotEmpty()
            .NotNull();
    }
}
