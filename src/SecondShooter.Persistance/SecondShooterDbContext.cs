using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecondShooter.Persistance.Entities;
using System.Reflection;

namespace SecondShooter.Persistance;

public class SecondShooterDbContext : DbContext
{
    public SecondShooterDbContext(
        DbContextOptions<SecondShooterDbContext> options
        ) : base(options)
    {
    }

    public DbSet<FileEntry> FileEntries { get; set; }
    public DbSet<FileContent> FileContents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var builderMethod = modelBuilder.GetType().GetMethod(nameof(ModelBuilder.Entity), 1, Type.EmptyTypes) ?? throw new NotSupportedException();

        var props = this.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
            .Where(p => p.PropertyType.IsGenericType)
            .Where(p => p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            ;
        foreach (var prop in props)
        {
            var entityType = prop.PropertyType.GetGenericArguments()[0];
            var entityTypeBuilder = typeof(EntityTypeBuilder<>).MakeGenericType(entityType);
            var onModelCreating = entityType.GetMethod(nameof(OnModelCreating), BindingFlags.Static | BindingFlags.Public, [entityTypeBuilder]);
            if (onModelCreating != null)
            {
                var entityBuilder = builderMethod.MakeGenericMethod(entityType).Invoke(modelBuilder, null) ?? throw new NotSupportedException();
                onModelCreating.Invoke(null, [entityBuilder]);
            }
        }
    }

    //public static void OnModelCreating(EntityTypeBuilder<TEntity> entity)
    //{
    //    entity.Property(e => e.HistoryId).ValueGeneratedNever();
    //}
}
