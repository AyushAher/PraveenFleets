using Domain.Common;

namespace Domain.Account;

public interface IAuditableEntity : IEntity
{
    Guid CreatedBy { get; set; }

    DateTime CreatedOn { get; set; }

    Guid? LastModifiedBy { get; set; }

    DateTime? LastModifiedOn { get; set; }

    Guid? DeletedBy { get; set; }

    DateTime? DeletedOn { get; set; }
}
