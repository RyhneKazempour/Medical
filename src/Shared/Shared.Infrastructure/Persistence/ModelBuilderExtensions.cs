namespace MyApp.Shared.Infrastructure.Persistence;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyApp.Shared.Domain;

public static class ModelBuilderExtensions
{
    public static void ApplySoftDeleteQueryFilters(this ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(ISoftDeletable).IsAssignableFrom(t.ClrType))
            .ToList();

        foreach (var entityType in entityTypes)
        {
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var notDeleted = Expression.Not(Expression.Equal(property, Expression.Constant(true)));
            var lambda = Expression.Lambda(notDeleted, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }
}
