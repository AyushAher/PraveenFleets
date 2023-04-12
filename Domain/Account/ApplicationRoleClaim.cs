using Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Domain.Account
{
    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>, IEntity
    {
        public Guid CreatedBy { get; set; } = Guid.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public Guid? LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public Guid? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

        public string? Description { get; set; } = string.Empty;

        public string? Group { get; set; }

        public override Guid RoleId
        {
            get => base.RoleId;
            set => base.RoleId = value;
        }

        public virtual ApplicationRole? Role { get; set; }

        public ApplicationRoleClaim()
        {
        }

        public ApplicationRoleClaim(string? roleClaimDescription = null, string? roleClaimGroup = null)
        {
            Description = roleClaimDescription;
            Group = roleClaimGroup;
        }
    }
}
