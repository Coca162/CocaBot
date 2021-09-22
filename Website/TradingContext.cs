using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using static Resources;
using static Shared.Main;

public class TraderBotContext : DbContext
{
    public DbSet<TraderEntity> Entities { get; set; }
    public DbSet<Deal> Deals { get; set; }

    public readonly static MySqlServerVersion version = new("8.0.26");
    public readonly static string ConnectionString = $@"server={config.Server};userid={config.UserID};password={config.Password};database=Trading";

    public TraderBotContext(DbContextOptions<TraderBotContext> options) : base(options)
    {

    }
}

public class TraderEntity
{
    public string Name { get; set; }
    public ulong Id { get; set; }

    public TraderEntity(string name, ulong id)
    {
        Name = name;
        Id = id;
    }
}

public class Deal
{
    [Key]
    public int Count { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public DateTime? Duration { get; set; }
    public Resource Resource { get; set; }
    public int Amount { get; set; }

    public Deal(string to, string from, bool oneTime, Resource resource, int amount)
    {
        To = to;
        From = from;
        Duration = oneTime ? null : DateTime.Today.AddDays(1);
        Resource = resource;
        Amount = amount;
    }

    private Deal(int count, string to, string from, DateTime? duration, Resource resource, int amount)
    {
        Count = count;
        To = to;
        From = from;
        Duration = duration;
        Resource = resource;
        Amount = amount;
    }
}