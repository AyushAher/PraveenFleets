using Microsoft.AspNetCore.Identity;

namespace Domain.Account
{

    public class ApplicationUserRole : IdentityUserRole<Guid>, IAuditableEntity
    {
        public Guid CreatedBy { get; set; } = Guid.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public Guid? LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public Guid? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }

    }
}
