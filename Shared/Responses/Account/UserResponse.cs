namespace Shared.Responses.Account;

public class UserResponse
{
    public string Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; }

    public bool IsActive { get; set; } = true;

    public string PhoneNumber { get; set; } = string.Empty;

}