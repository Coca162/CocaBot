﻿using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using static Shared.Main;

namespace Shared
{
    public class CocaBotContext : DbContext
    {
        public DbSet<Tokens> Tokens { get; set; }

        public readonly static string ConnectionString = $@"server={config.Server};userid={config.UserID};password={config.Password};database={config.Database}";
        public readonly static MySqlServerVersion version = new("8.0.25");

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseMySql(ConnectionString, version);
    }

    public class Tokens
    {
        [Key]
        public string SVID { get; set; }
        public string Token { get; set; }
        public string VerifKey { get; set; }
        public ulong? Valour { get; set; }
        public ulong? Discord { get; set; }
        public string ValourName { get; set; }
    }
}