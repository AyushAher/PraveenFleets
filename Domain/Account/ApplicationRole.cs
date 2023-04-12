using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Account;

public class ApplicationRole : IdentityRole<Guid>, IAuditableEntity
{
    public Guid CreatedBy { get; set; } = Guid.Empty;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    [MaxLength(25)]
    public override string Name
    {
        get => base.Name;
        set => base.Name = value;
    }

    [MaxLength(25)]
    public override string NormalizedName
    {
        get => base.NormalizedName;
        set => base.NormalizedName = value;
    }


    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;

    public virtual ICollection<ApplicationUserRole>? UserRoles { get; set; }

    public virtual ICollection<ApplicationRoleClaim>? RoleClaims { get; set; }

    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName, string? roleDescription = null)
        : base(roleName)
    {
        Description = roleDescription;
    }
}