using Microsoft.EntityFrameworkCore;
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
