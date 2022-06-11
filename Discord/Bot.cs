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
using DSharpPlus.Entities;
using static Discord.Events.MessageEvents;
using static Discord.Events.TimedEvents;
using static Discord.Program;
using static Shared.HttpClientExtensions;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using DSharpPlus.EventArgs;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace Discord;
public class Bot
{
    public static DiscordClient Client { get; private set; }

    public static IServiceProvider ServiceProvider { get; private set; }

    public static async Task RunAsync(string token, string[] prefixes)
    {
        DiscordConfiguration config = new()
        {
            Token = token,
            TokenType = TokenType.Bot,
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
            Intents = DiscordIntents.Guilds | DiscordIntents.AllUnprivileged
        };

        var client = new DiscordClient(config);

        client.Intents.AddIntent(DiscordIntents.GuildMembers);


        ServiceCollection services = new();

        services.AddDbContextPool<CocaBotPoolContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(CocaBotPoolContext.ConnectionString);
        });

        services.AddHttpClient();

        services.AddHttpClient<IUbiUserAPI, UbiUserAPI>();

        services.AddSingleton(client);

        //services.AddTransient<IUbiUsers>(provider => new UbiUsers(provider.GetRequiredService<HttpClient>()));

        services.AddSingleton<IUbiRoles<ulong>>(provider => new UbiRoles(provider.GetRequiredService<DiscordClient>()));

        ServiceProvider = services.BuildServiceProvider();

        CommandsNextConfiguration commandsConfig = new()
        {
            StringPrefixes = prefixes,
            Services = ServiceProvider
        };

        client.SetUpEvents();

        var commandsNext = client.UseCommandsNext(commandsConfig);

        commandsNext.SetHelpFormatter<HelpFormatter>();

        commandsNext.RegisterCommands(Assembly.GetExecutingAssembly());

        await client.ConnectAsync();
    }
}