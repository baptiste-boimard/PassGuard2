using Microsoft.EntityFrameworkCore;
using PassGuard.Shared.Models;

namespace PassGuard.Api.Database;

public class SqliteDbContext : DbContext
{
    public SqliteDbContext(DbContextOptions<SqliteDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Account> Accounts { get; set; }
    public DbSet<ObjectPassword> ObjectPasswords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Username).IsRequired();
            entity.Property(a => a.Password).IsRequired();
            entity.Property(a => a.Salt);
            entity.Property(a => a.CreatedAt);
        });

        modelBuilder.Entity<ObjectPassword>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Site).IsRequired();
            entity.Property(o => o.Username).IsRequired();
            entity.Property(o => o.Password).IsRequired();
            entity.Property(o => o.Category).IsRequired();
            entity.Property(o => o.CreatedAt);

            entity.HasOne(o => o.Account)
                .WithMany(a => a.ObjectPasswords)
                .HasForeignKey(o => o.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
    
}