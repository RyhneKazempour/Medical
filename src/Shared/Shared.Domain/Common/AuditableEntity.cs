namespace MyApp.Shared.Domain;

public interface IAuditableEntity
{
    DateTimeOffset CreatedAt { get; set; }
    Guid? CreatedUserId { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
    Guid? UpdatedUserId { get; set; }
    bool IsDeleted { get; set; }
}

public interface IActivatableEntity
{
    bool IsActive { get; set; }
}

public interface ISoftDeletable
{
    bool IsDeleted { get; }
}

public abstract class AuditableEntity : Entity, IAuditableEntity, ISoftDeletable
{
    protected AuditableEntity() { }

    protected AuditableEntity(Guid id) : base(id) { }

    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedUserId { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedUserId { get; set; }
    public bool IsDeleted { get; set; }
}

public abstract class AuditableActivatableEntity : AuditableEntity, IActivatableEntity
{
    protected AuditableActivatableEntity() { }

    protected AuditableActivatableEntity(Guid id) : base(id) { }

    public bool IsActive { get; set; } = true;
}
