using DSharpPlus;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Discord.Commands;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Timers;
using SpookVooper.Api;
using SpookVooper.Api.Entities;
using DSharpPlus.Entities;
using static Discord.TimedEvents;
using static Discord.Program;
using static SpookVooper.Api.SpookVooperAPI;

namespace Discord;
public class Bot
{
    public static DiscordClient Client { get; private set; }
    public static CommandsNextExtension Commands { get; private set; }
    public static async Task RunAsync(DiscordConfig ConfigJson)
    {
        DiscordConfiguration config = new()
        {
            Token = ConfigJson.Token,
            TokenType = TokenType.Bot,
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug
        };

        Client = new DiscordClient(config);
        Client.Ready += OnClientReady;

        Client.MessageCreated += async (s, e) =>
        {
            await GetData($"https://ubi.vtech.cf/new_message?id={e.Author.Id}&name={e.Author.Username}&key={ConfigJson.JacobUBIKey}");
        };

        CommandsNextConfiguration commandsConfig = new()
        {
            StringPrefixes = ConfigJson.Prefix,
            Services = new ServiceCollection().AddDbContextPool<CocaBotWebContext>((serviceProvider, options) =>
            {
                options.UseMySql(CocaBotWebContext.ConnectionString, CocaBotWebContext.version);
            }).BuildServiceProvider()
        };

        Commands = Client.UseCommandsNext(commandsConfig);

        Commands.SetHelpFormatter<HelpFormatter>();

        Commands.RegisterCommands(Assembly.GetExecutingAssembly());

        await Client.ConnectAsync();
    }

    private static async Task OnClientReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
    {
        Console.WriteLine("CocaBot on!");
        if (prod) await SetTimer();    
    }
}
