using Domain.MasterData;
using FluentValidation;

namespace ApplicationServices.MappingProfile.MasterData;

public class ListTypeValidators : AbstractValidator<ListType>
{
    public ListTypeValidators()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("List Code should not be empty.")
            .NotNull()
            .WithMessage("List Code should not be NULL.")
            .MaximumLength(8)
            .WithMessage("List Code should not be more than 8 characters long.")
            .MinimumLength(4)
            .WithMessage("List Code should be at least 4 characters long.");

        RuleFor(x => x.ListName)
            .NotEmpty()
            .WithMessage("List Name should not be empty.")
            .NotNull()
            .WithMessage("List Name should not be NULL.")
            .MaximumLength(150)
            .WithMessage("List Name should not be more than 150 characters long.")
            .MinimumLength(4)
            .WithMessage("List Name should be at least 4 characters long.");

    }
}