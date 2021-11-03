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
                if (prod) await SetTimer();
            });
        };

        Client.MessageCreated += async (s, e) =>
        {
            _ = Task.Run(async () =>
            {
                await GetData($"https://ubi.vtech.cf/new_message?id={e.Author.Id}&name={e.Author.Username}&key={ConfigJson.JacobUBIKey}");

            });
        };

        Task task = Task.Run(async () => UpdateHourly(ConfigJson));

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

    public static async Task UpdateHourly(DiscordConfig config)
    {
        while (true)
        {
            JacobUBIUserDataForHourlyUpdateData data = null;

            try
            {
                data = await GetDataFromJson<JacobUBIUserDataForHourlyUpdateData>($"https://ubi.vtech.cf/all_user_data?key={config.JacobUBIKey}");
            }
            catch
            {

                // wait 5s before trying again

                await Task.Delay(5000);

                continue;
            }
            

            DiscordGuild server = await Client.GetGuildAsync(798307000206360588);

            List<DiscordRole> SVRoles = new List<DiscordRole>();

            SVRoles.Add(server.GetRole(894632235326656552));
            SVRoles.Add(server.GetRole(894632423776731157));
            SVRoles.Add(server.GetRole(894632541552791632));
            SVRoles.Add(server.GetRole(894632641477894155));
            SVRoles.Add(server.GetRole(894632682330423377));

            foreach (JacobUBIUserDataForHourlyUpdateItem item in data.Users)
            {

                DiscordMember member = await server.GetMemberAsync(item.Id);

                if (member == null)
                {
                    continue;
                }

                // check if unranked

                if (item.Rank == "Unranked")
                {
                    bool HasRole = false;
                    DiscordRole RoleToRemove = null;
                    foreach (DiscordRole role in SVRoles)
                    {
                        if (member.Roles.Contains(role))
                        {
                            HasRole = true;
                            RoleToRemove = role;
                            break;
                        }
                    }
                    if (HasRole)
                    {
                        await member.RevokeRoleAsync(RoleToRemove);
                    }
                }

                else
                {
                    foreach (DiscordRole role in SVRoles)
                    {
                        if (member.Roles.Contains(role) && role.Name != item.Rank)
                        {
                            await member.RevokeRoleAsync(role);
                            break;
                        }
                    }

                    DiscordRole ToHave = SVRoles.Find(x => x.Name == item.Rank);

                    if (!member.Roles.Contains(ToHave))
                    {
                        await member.GrantRoleAsync(ToHave);
                    }

                }

            }

            // check every 30 minutes
            await Task.Delay(60*30*1000);
        }
    }
}

public class JacobUBIUserDataForHourlyUpdateItem
{
    [JsonPropertyName("Id")]
    public ulong Id { get; set; }

    [JsonPropertyName("Rank")]
    public string Rank { get; set; }
}

public class JacobUBIUserDataForHourlyUpdateData
{
    [JsonPropertyName("Users")]
    public List<JacobUBIUserDataForHourlyUpdateItem> Users { get; set; }
}