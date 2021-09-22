using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SpookVooper.Api;
using static Shared.Main;

namespace Shared;
public class CocaBotWebContext : DbContext
{
    public DbSet<Users> Users { get; set; }
    public DbSet<Registers> Registers { get; set; }

    public readonly static string ConnectionString = $@"server={config.Server};userid={config.UserID};password={config.Password};database={config.Database}";
    public readonly static MySqlServerVersion version = new("8.0.26");

    public CocaBotWebContext(DbContextOptions<CocaBotWebContext> options) : base(options)
    {

    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseMySql(ConnectionString, version, options => options.EnableRetryOnFailure());
}

public class CocaBotContext : DbContext
{
    public DbSet<Users> Users { get; set; }
    public DbSet<Registers> Registers { get; set; }

    public static readonly string ConnectionString = $@"server={config.Server};userid={config.UserID};password={config.Password};database={config.Database}";
    public static readonly MySqlServerVersion version = new("8.0.26");
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseMySql(ConnectionString, version, options => options.EnableRetryOnFailure());
}

public class Users
{
    [Key]
    public string SVID { get; set; }
    public string Token { get; set; }
    public ulong? Valour { get; set; }
    public ulong? Discord { get; set; }
    public string ValourName { get; set; }
}

public class Registers
{
    [Key]
    public string VerifKey { get; set; }
    public ulong Discord { get; set; }
}

public enum ApplicableTax
{
    None = 0, Corporate = 1, Payroll = 2, CapitalGains = 3, Sales = 4
}

public class Transaction
{
    public string FromAccount { get; set; }
    public string ToAccount { get; set;  }
    public decimal Amount { get; set; }
    public string Detail { get; set; }
    public bool Force { get; set; }
    public bool IsCompleted { get; set; }
    public ApplicableTax Tax { get; set; }
    ////TaskResult
    //public string Info { get; set; }
    //public bool Succeeded { get; set; }
    public TaskResult Result { get; set; }
}