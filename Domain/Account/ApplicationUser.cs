using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Enums.Account;
using Microsoft.AspNetCore.Identity;

namespace Domain.Account;

public class ApplicationUser : IdentityUser<Guid>, IAuditableEntity
{
    private string _fName = string.Empty;
    private string _lName = string.Empty;

    public Guid CreatedBy { get; set; } = Guid.Empty;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    [MaxLength(256)]
    public override string UserName
    {
        get => base.UserName;
        set => base.UserName = value.ToLower();
    }

    [MaxLength(256)]
    public override string NormalizedUserName
    {
        get => base.NormalizedUserName;
        set => base.NormalizedUserName = value;
    }

    [MaxLength(256)]
    public override string? Email
    {
        get => base.Email;
        set => base.Email = value;
    }

    [MaxLength(256)]
    public override string? NormalizedEmail
    {
        get => base.NormalizedEmail;
        set => base.NormalizedEmail = value;
    }

    [MaxLength(20)]
    public override string? PhoneNumber
    {
        get => base.PhoneNumber;
        set => base.PhoneNumber = value;
    }


    [Required]
    [StringLength(60, MinimumLength = 1, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string FirstName
    {
        get => _fName;
        set => _fName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
    }

    [Required]
    [StringLength(60, MinimumLength = 1, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string LastName
    {
        get => _lName;
        set => _lName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public string FullName => FirstName + " " + LastName;

    [MaxLength(100)]
    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

    public ApplicationUser() => UserRoles = new HashSet<ApplicationUserRole>();
    
    public Guid ParentEntityId { get; set; }

    public UserType UserType { get; set; }
}