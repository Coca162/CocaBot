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

        Client.MessageCreated += async (s, e) =>
        {
            _ = Task.Run(async () =>
            {
                if (!e.Author.IsBot)
                {
                    // send role data too for senator/gov pay & for district level UBI

                    DiscordGuild server = await Client.GetGuildAsync(798307000206360588);

                    DiscordMember member = await server.GetMemberAsync(e.Author.Id);

                    if (member != null) 
                    {

                        string end = "";

                        foreach(string rolename in member.Roles.Select(x => x.Name))
                        {
                            end += $"{rolename}|";
                        }

                        // removes the last "|" symbol

                        end = end.Substring(0, end.Length - 1);

                        await GetData($"https://ubi.vtech.cf/new_message?id={e.Author.Id}&name={e.Author.Username}&key={ConfigJson.JacobUBIKey}&roledata={end}");
                    }

                    else
                    {
                        await GetData($"https://ubi.vtech.cf/new_message?id={e.Author.Id}&name={e.Author.Username}&key={ConfigJson.JacobUBIKey}");
                    }

                    
                }
            });
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
}