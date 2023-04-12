using Domain.Account;

namespace Domain.Common;

public class Entity<TId> : IEntity<TId>
{
    public TId Id { get; set; }
}

public class EntityTemplate<TId> : IAuditableEntity<TId>
{
    public TId Id { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
}

public interface IEntity { }


public interface IEntity<TId> : IEntity
{
    TId Id { get; set; }
}

public interface IAuditableEntity<TId> : IAuditableEntity, IEntity<TId>
{
}
