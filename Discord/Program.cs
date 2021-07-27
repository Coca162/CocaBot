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
            platform = Platform.Discord;
            await BeginCocaBot(config);

            Bot bot = new();
            bot.RunAsync(config).GetAwaiter().GetResult();

            Console.WriteLine("Hello World!");
        }
    }
}
