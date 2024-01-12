using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SecondShooter.Persistance.Entities;

namespace SecondShooter.Persistance;

public class SecondShooterDbContext : DbContext
{
    public SecondShooterDbContext(
        DbContextOptions<SecondShooterDbContext> options
        ) : base(options)
    {
    }

    public DbSet<ImageFile> ImageFiles { get; set; }
}

public class SecondShooterDbContextFactory : IDesignTimeDbContextFactory<SecondShooterDbContext>
{
    public SecondShooterDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SecondShooterDbContext>();
        optionsBuilder.UseSqlServer(@"Server=(localdb)\SecondShooter;Data Source=SecondShooter");

        return new SecondShooterDbContext(optionsBuilder.Options);
    }
}