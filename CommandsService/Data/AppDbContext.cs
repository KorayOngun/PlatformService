using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data;

public class AppDbContext : DbContext
{
    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Command> Commands { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Platform>(entity =>
        {
            entity.HasMany(p => p.Commands).WithOne(c => c.Platform).HasForeignKey(c => c.PlatformId);
        });

        modelBuilder.Entity<Command>(entity =>
        {
            entity.HasOne(c => c.Platform).WithMany(p => p.Commands).HasForeignKey(c => c.PlatformId);
        });

    }
}