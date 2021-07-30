using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using static Shared.Main;

namespace Discord
{
    class Program
    {
        public static DiscordConfig Config;
        public static async Task Main(string[] args)
        {
            Config = await GetConfig<DiscordConfig>();
            platform = Platform.Discord;
            await BeginCocaBot(Config);

            Console.WriteLine("Hello World!");
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    });
    }
}