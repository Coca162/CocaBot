using Shared;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Shared.Main;

namespace Discord;
class Program
{
    public static bool prod;
    public static string UBIKey = "";

    public static async Task Main()
    {
        DiscordConfig config = await GetConfig<DiscordConfig>();
        prod = config.Production;
        UBIKey = config.JacobUBIKey;

        platform = Platform.Discord;

        await Bot.RunAsync(config.Token, config.Prefix);

        await Task.Delay(-1);
    }
}