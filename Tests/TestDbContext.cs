using Microsoft.EntityFrameworkCore;
using PassGuard.Shared.Models;

namespace Tests;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options){ }
    public DbSet<Account> Accounts { get; set; }
}