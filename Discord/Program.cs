using SpookVooper.Api.Entities;
using System;
using System.Threading.Tasks;
using static Shared.Main;

namespace Discord
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DiscordConfig config = await GetConfig<DiscordConfig>();
            await BeginMysql(config);

            Bot bot = new Bot();
            bot.RunAsync(config).GetAwaiter().GetResult();

            Console.WriteLine("Hello World!");
        }
    }
}
