using Domain.MasterData;
using FluentValidation;

namespace ApplicationServices.MappingProfile.MasterData;

public class ListTypeItemValidators : AbstractValidator<ListTypeItem>
{
    public ListTypeItemValidators()
    {
        RuleFor(x => x.Code)
            .MaximumLength(8)
            .WithMessage("Item Code should not be more than 8 characters long.")
            .MinimumLength(4)
            .WithMessage("Item Code should be at least 4 characters long.")
            .NotEmpty()
            .WithMessage("Item Code should not be Empty.");

        RuleFor(x => x.ItemName)
            .MaximumLength(150)
            .WithMessage("Item Name should not be more than 150 characters long.")
            .MinimumLength(5)
            .WithMessage("Item Name should be at least 5 characters long.")
            .NotEmpty()
            .WithMessage("Item Name should not be Empty.");

        RuleFor(x => x.ListTypeId)
            .NotEmpty()
            .WithMessage("List Type is Empty.")
            .NotNull()
            .WithMessage("List Type is null");

    }
}