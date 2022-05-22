using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Models;
using Shared.Models.Database;
using static Shared.Main;

namespace Shared;


public class CocaBotPoolContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public static readonly string ConnectionString = $@"Host={config.Server};Username={config.UserID};Password={config.Password};Database={config.Database}";

    public CocaBotPoolContext(DbContextOptions<CocaBotPoolContext> options) : base(options) { }
}

public class CocaBotContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public static readonly string ConnectionString = $@"Host={config.Server};Username={config.UserID};Password={config.Password};Database={config.Database}";
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseNpgsql(ConnectionString);
}