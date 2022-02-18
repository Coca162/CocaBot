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
using static Discord.Events.MessageEvents;
using static Discord.Events.TimedEvents;
using static Discord.Program;
using static SpookVooper.Api.SpookVooperAPI;
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
    public static async Task RunAsync(string token, string[] prefixes)
    {
        DiscordConfiguration config = new()
        {
            Token = token,
            TokenType = TokenType.Bot,
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
            Intents = DiscordIntents.GuildMembers | DiscordIntents.Guilds | DiscordIntents.GuildMessages
        };

        Client = new DiscordClient(config);

        Client.Intents.AddIntent(DiscordIntents.GuildMembers);

        SetUpEvents();

        CommandsNextConfiguration commandsConfig = new()
        {
            StringPrefixes = prefixes,
            Services = new ServiceCollection().AddDbContextPool<CocaBotWebContext>((serviceProvider, options) =>
            {
                options.UseMySql(CocaBotWebContext.ConnectionString, CocaBotWebContext.version);
            }).BuildServiceProvider()
        };

        var commandsNext = Client.UseCommandsNext(commandsConfig);

        commandsNext.SetHelpFormatter<HelpFormatter>();

        commandsNext.RegisterCommands(Assembly.GetExecutingAssembly());

        await Client.ConnectAsync();
    }

    private static void SetUpEvents()
    {
        Client.Ready += async (sender, e) =>
        {
            Console.WriteLine("CocaBot on!");
        };

        if (!prod) return;

        Client.Ready += (s, e) =>
        {
            Task.Run(async () =>
            {
                ServiceWrapper wrapper = new();
                Task.Run(async () => await wrapper._ServiceWrapper(s));
                await SetTimer();
            });
            return Task.CompletedTask;
        };

        Client.MessageCreated += (s, args) =>
        {
            Task.Run(async () => await HandleMessage(UBIKey, args));
            return Task.CompletedTask;
        };

        if (!File.Exists("moi.json")) return;

        var rng = new Random();
        Events.MemberJoinEvents.NextHelper = rng.Next(0, Events.MemberJoinEvents.Helpers.Count - 1);

        Client.GuildMemberAdded += (s, args) =>
        {
            Task.Run(async () => await Events.MemberJoinEvents.HandleSVJoin(args));
            return Task.CompletedTask;
        };
    }
}

public class ServiceWrapper
{
    HttpClient httpclient { get; set; }

    DiscordChannel channel { get; set; }

    public async Task _ServiceWrapper(DiscordClient client)
    {
        httpclient = new HttpClient();
        channel = await client.GetChannelAsync(908560388923220018);
        while (true)
        {
            await ServerEventOccursAsync(await httpclient.GetStreamAsync(new Uri("https://nvse.vtech.cf/stream_cocabot")));
        }
    }

    private async Task ServerEventOccursAsync(Stream s)
    {
        using var sr = new StreamReader(s);
        string message = await sr.ReadLineAsync();

        if (!message.Contains("ping"))
        {
            message = message.Replace("data: ", "");
            channel.SendMessageAsync(message);
        }
    }
}