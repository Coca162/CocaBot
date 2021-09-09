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

        Client.Ready += OnClientReady; ;

        CommandsNextConfiguration commandsConfig = new()
        {
            StringPrefixes = ConfigJson.Prefix
        };

        Commands = Client.UseCommandsNext(commandsConfig);

        //Commands here

        Commands.RegisterCommands<Economy>();

        Commands.RegisterCommands<Misc>();

        Commands.RegisterCommands<Valour>();

        Commands.RegisterCommands<Stats>();

        await Client.ConnectAsync();

        await Task.Delay(-1);
    }

    private Task OnClientReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e) => Task.CompletedTask;
}
