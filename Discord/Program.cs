using System;
using System.Threading;
using System.Threading.Tasks;
using static Shared.Main;

namespace Discord;
class Program
{
    public static bool prod;

    public static async Task Main(string[] args)
    {
        DiscordConfig config = await GetConfig<DiscordConfig>();
        prod = config.Production;
        if (config.Production) await LoadSVIDNameCache();
        platform = Platform.Discord;

        await Bot.RunAsync(config);

        await Task.Delay(-1);
    }
}
