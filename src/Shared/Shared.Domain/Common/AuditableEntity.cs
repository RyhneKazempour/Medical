namespace MyApp.Shared.Domain;

public interface IAuditableEntity
{
    DateTimeOffset CreatedAt { get; set; }
    int? CreatedUserId { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
    int? UpdatedUserId { get; set; }
    bool IsDeleted { get; set; }
}

public interface IActivatableEntity
{
    bool IsActive { get; set; }
}

public abstract class AuditableEntity : Entity, IAuditableEntity
{
    protected AuditableEntity() { }

    protected AuditableEntity(Guid id) : base(id) { }

    public DateTimeOffset CreatedAt { get; set; }
    public int? CreatedUserId { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public int? UpdatedUserId { get; set; }
    public bool IsDeleted { get; set; }
}

public abstract class AuditableActivatableEntity : AuditableEntity, IActivatableEntity
{
    protected AuditableActivatableEntity() { }

    protected AuditableActivatableEntity(Guid id) : base(id) { }

    public bool IsActive { get; set; } = true;
}
