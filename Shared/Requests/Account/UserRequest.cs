using FluentValidation;

namespace Shared.Requests.Account;

public class UserRequest
{
    public string? Id { get; set; }
    public static string CollectionLabel { get; set; } = "User";
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public bool EmailVerified { get; set; }
    public string? Name { get; set; }
}

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {

        RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress();

        RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull();

        RuleFor(x => x.UserName)
                .NotEmpty()
                .NotNull();

        RuleFor(x => x.Password)
                .NotEmpty()
                .NotNull();
    }
}