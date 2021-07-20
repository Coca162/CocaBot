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
            await BeginCocaBot(config, Platform.Discord);

            Bot bot = new Bot();
            bot.RunAsync(config).GetAwaiter().GetResult();

            Console.WriteLine("Hello World!");
        }
    }
}
