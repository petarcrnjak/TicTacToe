using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Authorization;
using TicTacToe.Database.Models;

namespace TicTacToe.Database;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Games> Games { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Optional: customize Identity table names or schema if needed
        // builder.Entity<AppUser>(b => b.ToTable("Users"));
        builder.Entity<Games>(entity =>
        {
            entity.ToTable("Games")
            .Property(g => g.Status)
            .HasComment("GameStatus: 0=Open, 1=InProgress, 2=Finished");

            entity.HasOne<AppUser>()
                   .WithMany()
                   .HasForeignKey(g => g.PlayerX)
                   .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<AppUser>()
                  .WithMany()
                  .HasForeignKey(g => g.PlayerO)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}