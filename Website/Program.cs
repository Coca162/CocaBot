using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using static Shared.Database;
using static Shared.CocaBotWebContext;
using static Shared.Main;
using static Shared.Commands.Balance;
using System;
using Newtonsoft.Json;
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

namespace Website
{
    public static class Program
    {
        public static string TotalMoney = "";
        public const double TotalMoneyInterval = 60 * 60 * 1000; //one hour

        public static async Task Main(string[] args)
        {
            WebsiteConfig config = await GetConfig<WebsiteConfig>();
            await BeginCocaBot(config);
            string baseUrl = null;

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEntityFrameworkMySql();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddDbContextPool<CocaBotWebContext>((serviceProvider, options) => {
                options.UseMySql(ConnectionString, version);
            });
            builder.Services.AddHttpClient();
            builder.Services.AddRazorPages();
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            //app.UseHttpsRedirection();

            app.Map("/login", async (HttpContext http, string key) => {
                if (baseUrl == null) baseUrl = http.Request.Scheme + "://" + http.Request.Host.ToString();
                Console.WriteLine(baseUrl);
                http.Response.Redirect($"https://spookvooper.com/oauth2/authorize?response_type=code&client_id={config.ClientId}&scope=view,eco&redirect_uri={baseUrl + "/callback"}&state={key}");
            });

            app.Map("/callback", async (HttpContext http, HttpClient client, CocaBotWebContext db, string code, string state) => {
                var response = await client.GetAsync($"https://spookvooper.com/oauth2/RequestToken?grant_type=authorization_code&code={code}&redirect_uri={baseUrl + "/callback"}&client_id={config.ClientId}&client_secret={config.ClientSecret}");
                if (!response.IsSuccessStatusCode) return "Spookvooper no work";
                TokenReturn tokenReturn = JsonConvert.DeserializeObject<TokenReturn>(await response.Content.ReadAsStringAsync());

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

            await SetTotalMoneyAsync().ConfigureAwait(false);
            Timer timer = new(TotalMoneyInterval);
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
                SpookVooper.Api.Entities.Entity entity = new(svid);
                decimal balance = entity.GetBalance();
                total += balance;
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

    public struct WebsiteConfig : DefaultConfig
    {
        [JsonProperty("clientsecret")]
        public string ClientSecret { get; private set; }
        [JsonProperty("clientid")]
        public string ClientId { get; private set; }
        [JsonProperty("server")]
        public string Server { get; private set; }
        [JsonProperty("userid")]
        public string UserID { get; private set; }
        [JsonProperty("password")]
        public string Password { get; private set; }
        [JsonProperty("database")]
        public string Database { get; private set; }
        [JsonProperty("oauth_secret")]
        public string OauthSecret { get; private set; }
    }

    public class TokenReturn
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; private set; }
        [JsonProperty("svid")]
        public string SVID { get; private set; }
        [JsonProperty("expires_in")]
        public int Expiry { get; private set; }
    }
}