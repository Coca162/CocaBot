using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using static Shared.Main;
using System;
using Shared;
using Microsoft.AspNetCore.Hosting;
using Website;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Microsoft.Extensions.Options;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Timers;
using System.Diagnostics;
using Humanizer;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using static Shared.HttpClientExtensions;
using Shared.Models.Database;

namespace Website;
public static class Program
{
    public const double TotalMoneyInterval = 60 * 60 * 1000; //one hour

    public static async Task Main(string[] args)
    {
        WebsiteConfig config = await GetConfig<WebsiteConfig>();

        CocaBotContext ds = new();
        ds.Database.EnsureCreated();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddDbContextPool<CocaBotPoolContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(CocaBotPoolContext.ConnectionString);
        });

        builder.Services.AddHttpClient();
        
        builder.Services.AddRazorPages();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // global cors policy
        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true) // allow any origin
            .AllowCredentials()); // allow credentials

        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });

        app.MapGet("/discord/getsvid", async (CocaBotPoolContext db, ulong id) => 
        {
            string svid = await db.Users.Where(x => x.Discord == id).Select(x => x.SVID).SingleOrDefaultAsync();
            return svid is not null ? svid : "";
        });

        app.MapGet("/valour/getsvid", async (CocaBotPoolContext db, ulong id) =>
        {
            string svid = await db.Users.Where(x => x.Valour == id).Select(x => x.SVID).SingleOrDefaultAsync();
            return svid is not null ? svid : "";
        });

        app.MapGet("/Transactions/Get", (CocaBotPoolContext db, int amount) =>
            db.Transactions.OrderByDescending(x => x.Count)
                                 .Take(amount));

        app.MapGet("/Transactions/GetFrom", async (CocaBotPoolContext db, string svid, int amount) =>
            await db.Transactions.Where(x => x.FromAccount == svid)
                                 .OrderByDescending(x => x.Count)
                                 .Take(amount)
                                 .ToListAsync());

        app.MapGet("/Transactions/GetTo", async (CocaBotPoolContext db, string svid, int amount) =>
            await db.Transactions.Where(x => x.ToAccount == svid)
                                 .OrderByDescending(x => x.Count)
                                 .Take(amount)
                                 .ToListAsync());

        app.MapGet("/Transactions/Filter", Filter);

        app.MapGet("/Transactions/SumFilter", SumFilter);

        app.Run();
    }

    static async Task<List<Transaction>> Filter(CocaBotPoolContext db, int amount = -1, string to = "", string from = "", string detail = "", int tax = -1, long start = -1, long end = -1) 
        => await GetTransactionsFilter(db, amount, to, from, detail, tax, start, end).ToListAsync();

    static async Task<decimal> SumFilter(CocaBotPoolContext db, int amount = -1, string to = "", string from = "", string detail = "", int tax = -1, long start = -1, long end = -1)
        => await GetTransactionsFilter(db, amount, to, from, detail, tax, start, end).SumAsync(x => x.Amount);

    static IQueryable<Transaction> GetTransactionsFilter(CocaBotPoolContext db, int proposedAmount, string to, string from, string detail, int tax, long start, long end)
    {
        IQueryable<Transaction> search = db.Transactions.AsQueryable();

        if (to != "") search = search.Where(x => x.ToAccount == to);
        if (from != "") search = search.Where(x => x.FromAccount == from);
        if (tax != -1) search = search.Where(x => (int)x.Tax == tax);
        if (detail != "") search = search.Where(x => x.Detail.Contains(detail));
        if (start != -1) search = search.Where(x => x.Timestamp > start);
        if (end != -1) search = search.Where(x => x.Timestamp < end);

        int amount = proposedAmount switch
        {
            -1 => 100,
            > 100000 => 100000,
            _ => proposedAmount
        };
     
        return search.OrderByDescending(x => x.Count).Take(amount);
    }
}

public class WebsiteConfig : DefaultConfig
{
    [JsonPropertyName("clientsecret")]
    public string ClientSecret { get; set; }
    [JsonPropertyName("clientid")]
    public string ClientId { get; set; }
}

public class TokenReturn
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("svid")]
    public string SVID { get; set; }
    [JsonPropertyName("expires_in")]
    public int Expiry { get; set; }
}