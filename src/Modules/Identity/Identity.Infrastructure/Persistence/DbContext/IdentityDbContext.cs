namespace MyApp.Identity.Infrastructure.Persistence.DbContext;

using Microsoft.EntityFrameworkCore;
using MyApp.Identity.Domain.Entities;
using MyApp.Identity.Infrastructure.Persistence.Configurations;
using MyApp.Shared.Application.Abstractions;
using MyApp.Shared.Domain;
using MyApp.Shared.Infrastructure.Persistence;

public class IdentityDbContext : DbContext, IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    private readonly ICurrentUser _currentUser;

    protected IdentityDbContext() : this(new DbContextOptions<IdentityDbContext>(), null!) { }

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, ICurrentUser currentUser) : base(options)
    {
        _currentUser = currentUser;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

        // Apply soft delete query filters automatically
        modelBuilder.ApplySoftDeleteQueryFilters();

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-populate audit fields
        var entries = ChangeTracker.Entries<IAuditableEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                if (_currentUser?.UserId.HasValue == true)
                {
                    entry.Entity.CreatedUserId = _currentUser.UserId;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                if (_currentUser?.UserId.HasValue == true)
                {
                    entry.Entity.UpdatedUserId = _currentUser.UserId;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}