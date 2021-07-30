using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using static Shared.Database;
using static Shared.CocaBotContext;
using static Shared.Main;
using System;
using Newtonsoft.Json;
using Shared;
using Microsoft.AspNetCore.Hosting;
using Website;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;

namespace Website
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebsiteConfig config = await GetConfig<WebsiteConfig>();
            await BeginCocaBot(config);
            string baseUrl = null;

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddDbContextPool<CocaBotContext>(options =>
            {
                options.UseMySql(ConnectionString, version, options => options.EnableRetryOnFailure());
            });
            builder.Services.AddHttpClient();
            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseSession();

            app.UseSession();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("/login", async (HttpContext http, string key) => {
                if (baseUrl == null) baseUrl = http.Request.Scheme + "://" + http.Request.Host.ToString();
                Console.WriteLine(baseUrl);
                http.Session.SetString("key", key);
                http.Response.Redirect($"https://spookvooper.com/oauth2/authorize?response_type=code&client_id={config.ClientId}&scope=view,eco&redirect_uri={baseUrl + "/callback"}");
            });

            app.Map("/callback", async (HttpContext http, HttpClient client, CocaBotContext db, string code, string state) => {
                string key = http.Session.GetString("key");
                if (key == null) return "this is null!";
                http.Session.Clear();

                var response = await client.GetAsync($"https://spookvooper.com/oauth2/RequestToken?grant_type=authorization_code&code={code}&redirect_uri={baseUrl + "/callback"}&client_id={config.ClientId}&client_secret={config.ClientSecret}");
                if (!response.IsSuccessStatusCode) return "no";
                TokenReturn tokenReturn = JsonConvert.DeserializeObject<TokenReturn>(await response.Content.ReadAsStringAsync());

                Tokens user = db.Tokens.Where(x => x.VerifKey == key).FirstOrDefault();
                user.SVID = tokenReturn.SVID;
                user.Token = tokenReturn.AccessToken;
                user.VerifKey = null;
                await db.SaveChangesAsync();

                http.Response.Redirect(baseUrl);
                return "";
            });

            app.Map("/", async (HttpContext http) => {
                return new ContentResult
                {
                    ContentType = "text/html",
                    Content = "<center><h1>You can close this page!</h1></center>"
                };
            });
            
            app.Run();
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