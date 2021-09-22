using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using static Shared.Database;
using static Shared.Main;
using static Shared.Commands.Balance;
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
using SpookVooper.Api.Entities;
using CsvHelper;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using SpookVooper.Api;

namespace Website;
public static class Program
{
    public static string TotalMoney = "";
    public const double TotalMoneyInterval = 60 * 60 * 1000; //one hour

    public static async Task Main(string[] args)
    {
        WebsiteConfig config = await GetConfig<WebsiteConfig>();
        await BeginCocaBot(config);
        string baseUrl = null;

        CocaBotContext ds = new();
        ds.Database.EnsureCreated();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEntityFrameworkMySql();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddDbContextPool<CocaBotWebContext>((serviceProvider, options) =>
        {
            options.UseMySql(CocaBotWebContext.ConnectionString, CocaBotWebContext.version);
        });

        builder.Services.AddDbContextPool<TraderBotContext>((serviceProvider, options) =>
        {
            options.UseMySql(TraderBotContext.ConnectionString, TraderBotContext.version);
        });

        builder.Services.AddHttpClient();

        builder.Services.AddRazorPages();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });

        app.Map("/login", async (HttpContext http, string key) =>
        {
            if (baseUrl == null) baseUrl = http.Request.Scheme + "://" + http.Request.Host.ToString();
            http.Response.Redirect($"https://spookvooper.com/oauth2/authorize?response_type=code&client_id={config.ClientId}&scope=view,eco&redirect_uri={baseUrl + "/callback"}&state={key}");
            http.Response.Redirect($"https://spookvooper.com/oauth2/authorize?response_type=code&client_id={config.ClientId}&scope=view,eco&redirect_uri={baseUrl + "/callback"}&state={key}");
        });

        app.Map("/callback", async (HttpContext http, HttpClient client, CocaBotWebContext db, string code, string state) =>
        {
            var response = await client.GetAsync($"https://spookvooper.com/oauth2/RequestToken?grant_type=authorization_code&code={code}&redirect_uri={baseUrl + "/callback"}&client_id={config.ClientId}&client_secret={config.ClientSecret}");
            if (!response.IsSuccessStatusCode) return "Spookvooper no work";
            TokenReturn tokenReturn = await JsonSerializer.DeserializeAsync<TokenReturn>(await response.Content.ReadAsStreamAsync());

            Registers register = await db.Registers.FindAsync(state);
            if (register == null) return "Discord bot no work";
            Users user = await db.Users.FindAsync(tokenReturn.SVID);
            if (user != null) db.Users.Remove(user);

            user = new();
            user.SVID = tokenReturn.SVID;
            user.Token = tokenReturn.AccessToken;
            user.Discord = register.Discord;

            await db.Users.AddAsync(user);
            db.Registers.Remove(register);

            await db.SaveChangesAsync();

            http.Response.Redirect(baseUrl + "/end");
            return "You can close this page!";
        });

        app.MapGet("/getsvid", async (HttpContext http, CocaBotWebContext db, ulong id, string type) =>
        {
            Users user;
            if (type == "d") user = db.Users.Where(x => x.Discord == id).FirstOrDefault();
            else if (type == "v") user = db.Users.Where(x => x.Valour == id).FirstOrDefault();
            else return "type is not an actual type!";

            if (user != null) return user.SVID;
            else return "User does not exist!";
        });

        app.MapGet("/deals", async (HttpContext http, TraderBotContext db) =>
        {
            var writer = new StringWriter();
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            
            foreach (var deal in db.Deals)
            {
                csv.WriteField(deal.From);
                csv.WriteField(deal.To);
                csv.WriteField(deal.Resource);
                csv.WriteField(deal.Amount);
                csv.WriteField(deal.Duration == null ? "" : ((DateTime)deal.Duration).ToString("dd/MM/yy", CultureInfo.InvariantCulture));

                csv.NextRecord();
            }

            return writer.ToString(); ;
        });

        SetTotalMoneyAsync();
        System.Timers.Timer timer = new(TotalMoneyInterval);
        timer.Elapsed += async (object source, ElapsedEventArgs e) => await SetTotalMoneyAsync();
        timer.Enabled = true;

        app.Run();
    }

    static async Task SetTotalMoneyAsync()
    {
        CocaBotContext db = new();

        decimal total = 0;
        List<string> svids = db.Users.Select(x => x.SVID).ToList();
        foreach (string svid in svids)
        {
            Entity entity = new(svid);
            TaskResult<decimal> balance = await entity.GetBalanceAsync();
            if (balance.Succeeded == false) return;
            total += balance.Data;
        }
        double scale = Math.Pow(10, Math.Floor(Math.Log10((double)total)) + 1);
        double rounded = scale * Math.Round((double)total / scale, 2);

        TotalMoney = Humanize(rounded);
    }

    public static string Humanize(this double number)
    {
        string[] suffix = { "f", "a", "p", "n", "μ", "m", string.Empty, " thousand", " million", " billion", " trillion", "P", "E" };

        int mag;
        if (number < 1)
        {
            mag = (int)Math.Floor(Math.Floor(Math.Log10(number)) / 3);
        }
        else
        {
            mag = (int)(Math.Floor(Math.Log10(number)) / 3);
        }

        var shortNumber = number / Math.Pow(10, mag * 3);

        return $"{shortNumber:0.###}{suffix[mag + 6]}";
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
