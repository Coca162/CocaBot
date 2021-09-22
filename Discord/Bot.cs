using DSharpPlus;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Discord.Commands;

namespace Discord;
public class Bot
{
    public DiscordClient Client { get; private set; }
    public CommandsNextExtension Commands { get; private set; }
    public async Task RunAsync(DiscordConfig ConfigJson)
    {
        DiscordConfiguration config = new()
        {
            Token = ConfigJson.Token,
            TokenType = TokenType.Bot,
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug
        };

        Client = new DiscordClient(config);

        CommandsNextConfiguration commandsConfig = new()
        {
            StringPrefixes = ConfigJson.Prefix
        };

        Commands = Client.UseCommandsNext(commandsConfig);

        Commands.SetHelpFormatter<HelpFormatter>();

        //Commands here

        Commands.RegisterCommands<Balance>();
        Commands.RegisterCommands<Connectivity>();
        Commands.RegisterCommands<Get>();
        Commands.RegisterCommands<Misc>();
        Commands.RegisterCommands<Pay>();
        Commands.RegisterCommands<Stats>();
        Commands.RegisterCommands<Valour>();

        await Client.ConnectAsync();

        await Task.Delay(-1);
    }
}
