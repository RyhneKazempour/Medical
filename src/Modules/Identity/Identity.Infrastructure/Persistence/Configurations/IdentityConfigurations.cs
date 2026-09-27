namespace MyApp.Identity.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Identity.Domain.Entities;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
        builder.Property(u => u.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        builder.Property(u => u.Phone).HasColumnName("phone").HasMaxLength(20);

        builder.Property(u => u.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(u => u.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(u => u.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");
        builder.Property(u => u.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("uq_users_email").HasFilter("is_deleted = false");
        builder.HasIndex(u => u.IsActive).HasDatabaseName("ix_users_is_active").HasFilter("is_deleted = false");
    }
}

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(r => r.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(r => r.Description).HasColumnName("description");
        builder.Property(r => r.ContextType).HasColumnName("context_type").HasMaxLength(50);
        builder.Property(r => r.ContextId).HasColumnName("context_id");

        builder.Property(r => r.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(r => r.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(r => r.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(r => r.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");
        builder.Property(r => r.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasIndex(r => r.Name).IsUnique().HasDatabaseName("uq_roles_name").HasFilter("is_deleted = false");
        builder.HasIndex(r => new { r.ContextType, r.ContextId }).HasDatabaseName("ix_roles_context").HasFilter("is_deleted = false");
    }
}

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(p => p.Resource).HasColumnName("resource").HasMaxLength(100).IsRequired();
        builder.Property(p => p.Action).HasColumnName("action").HasMaxLength(50).IsRequired();
        builder.Property(p => p.Description).HasColumnName("description");

        builder.Property(p => p.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(p => p.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasIndex(p => new { p.Resource, p.Action }).IsUnique().HasDatabaseName("uq_permissions_resource_action").HasFilter("is_deleted = false");
    }
}

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(ur => ur.Id);
        builder.Property(ur => ur.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(ur => ur.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(ur => ur.RoleId).HasColumnName("role_id").IsRequired();
        builder.Property(ur => ur.ScopeType).HasColumnName("scope_type").HasMaxLength(50);
        builder.Property(ur => ur.ScopeId).HasColumnName("scope_id");

        builder.Property(ur => ur.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(ur => ur.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(ur => ur.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(ur => ur.UpdatedAt).HasColumnName("updated_at");
        builder.Property(ur => ur.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasOne(ur => ur.User).WithMany().HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(ur => ur.Role).WithMany().HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ur => ur.UserId).HasDatabaseName("ix_user_roles_user_id").HasFilter("is_deleted = false");
        builder.HasIndex(ur => ur.RoleId).HasDatabaseName("ix_user_roles_role_id").HasFilter("is_deleted = false");
        builder.HasIndex(ur => new { ur.ScopeType, ur.ScopeId }).HasDatabaseName("ix_user_roles_scope").HasFilter("is_deleted = false");
    }
}

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(rp => rp.Id);
        builder.Property(rp => rp.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(rp => rp.RoleId).HasColumnName("role_id").IsRequired();
        builder.Property(rp => rp.PermissionId).HasColumnName("permission_id").IsRequired();

        builder.Property(rp => rp.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(rp => rp.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(rp => rp.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(rp => rp.UpdatedAt).HasColumnName("updated_at");
        builder.Property(rp => rp.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasOne(rp => rp.Role).WithMany().HasForeignKey(rp => rp.RoleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(rp => rp.Permission).WithMany().HasForeignKey(rp => rp.PermissionId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(rp => rp.RoleId).HasDatabaseName("ix_role_permissions_role_id").HasFilter("is_deleted = false");
        builder.HasIndex(rp => rp.PermissionId).HasDatabaseName("ix_role_permissions_permission_id").HasFilter("is_deleted = false");
    }
}
