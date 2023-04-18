using Enums.Account;

namespace Shared.Responses.Account;

public class UserResponse
{
    public Guid Id { get; set; }
    private string _fName = string.Empty;
    private string _lName = string.Empty;

    public Guid CreatedBy { get; set; } = Guid.Empty;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public string UserName { get; set; }

    public string NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public string? PhoneNumber { get; set; }


    public string FirstName
    {
        get => _fName;
        set => _fName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
    }

    public string LastName
    {
        get => _lName;
        set => _lName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
    }

    public string FullName => FirstName + " " + LastName;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public Guid ParentEntityId { get; set; }

    public UserType UserType { get; set; }

}