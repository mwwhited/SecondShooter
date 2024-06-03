using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SecondShooter.Persistance;

public class SecondShooterDbContextFactory : IDesignTimeDbContextFactory<SecondShooterDbContext>
{
    public SecondShooterDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SecondShooterDbContext>();
        optionsBuilder.UseSqlServer(@"Server=(localdb)\SecondShooter;Data Source=SecondShooter");

        return new SecondShooterDbContext(optionsBuilder.Options);
    }
}