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
using System.Text.Json.Serialization;
using System.Collections.Generic;
using DSharpPlus.EventArgs;

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

        Client.Intents.AddIntent(DiscordIntents.GuildMembers);

        Client.Ready += async (sender, e) =>
        {
            _ = Task.Run(async () =>
            {
                Console.WriteLine("CocaBot on!");
                if (prod) await SetTimer(ConfigJson);
            });
        };

        Client.MessageCreated += async (s, e) => HandleMessage(ConfigJson.JacobUBIKey, e);

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

    private static async Task HandleMessage(string ubiKey, MessageCreateEventArgs e)
    {
        if (!prod || e.Author.IsBot) return;

        // send role data too for senator/gov pay & for district level UBI

        DiscordGuild server = await Client.GetGuildAsync(798307000206360588);

        DiscordMember member = await server.GetMemberAsync(e.Author.Id);

        if (member is null)
        {
            GetData($"https://ubi.vtech.cf/new_message?id={e.Author.Id}&name={e.Author.Username}&key={ubiKey}");
            return;
        }

        string end = "";

        foreach (string rolename in member.Roles.Select(x => x.Name))
        {
            end += $"{rolename}|";
        }

        // removes the last "|" symbol
        end = end[0..^1];

        GetData($"https://ubi.vtech.cf/new_message?id={e.Author.Id}&name={e.Author.Username}&key={ubiKey}&roledata={end}");
    }
}