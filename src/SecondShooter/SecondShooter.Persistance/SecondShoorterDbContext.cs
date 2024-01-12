using Microsoft.EntityFrameworkCore;
using SecondShooter.Persistance.Entities;

namespace SecondShooter.Persistance;

public class SecondShoorterDbContext : DbContext
{
    public DbSet<ImageFile> ImageFiles { get; set; }
}
