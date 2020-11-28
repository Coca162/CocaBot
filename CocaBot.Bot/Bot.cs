using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CocaBot.Commands;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using SpookVooper.Api.Entities;
using System;

namespace CocaBot
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public async Task RunAsync()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            ConfigJson ConfigJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            DiscordConfiguration config = new DiscordConfiguration
            {
                Token = ConfigJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug
            };

#pragma warning disable IDE0003
            this.Client = new DiscordClient(config);
#pragma warning restore IDE0003

            Client.Ready += OnClientReady;
            Client.GuildMemberAdded += Client_GuildMemberAdded;

            CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { ConfigJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true,
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.CommandErrored += CmdErroredHandler;
            Commands.SetHelpFormatter<CustomHelpFormatter>();

            // Basic:
            Commands.RegisterCommands<Basic>();
            // Economy:
            Commands.RegisterCommands<Balance>();
            // Other:
            Commands.RegisterCommands<Verify>();
            // Users:
            Commands.RegisterCommands<Name>();
            Commands.RegisterCommands<Statistics>();
            Commands.RegisterCommands<SVID>();
            // XP:
            Commands.RegisterCommands<Experience>();
            Commands.RegisterCommands<Leaderboards>();

            // Loop variations of commands are included in the same command file

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private async Task CmdErroredHandler(CommandsNextExtension _, CommandErrorEventArgs e)
        {
            try
            {
                var failedChecks = ((ChecksFailedException)e.Exception).FailedChecks;
                foreach (var failedCheck in failedChecks)
                {
                    if (failedCheck is EnableBlacklist)
                    {
                        await e.Context.RespondAsync($"You are blacklisted!");
                    }
                    if (failedCheck is DeveloperOnly)
                    {
                        await e.Context.RespondAsync($"You are not a whitelisted developer!");
                    }
                }
            }
            catch (Exception a)
            {
                await e.Context.RespondAsync($"While attempting to run the command the following error has happened:\n{e.Exception.Message}");
            }
        }

        private async Task Client_GuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            string json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            ConfigJson ConfigJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            ulong GuildID = e.Member.Guild.Id;
            if (GuildID == ConfigJson.ServerID)
            {
                ulong discordID = e.Member.Id;
                User user = new User(await User.GetSVIDFromDiscordAsync(discordID));
                var data = await user.GetSnapshotAsync();
                DiscordRole district_role = e.Member.Guild.GetRole(ConfigJson.CitizenID);
                DiscordRole non_citizen_role = e.Member.Guild.GetRole(ConfigJson.NonCitizenID);
                DiscordRole unpicked_state_role = e.Member.Guild.GetRole(778423688118272071);
                string senate_role = "Senator";
                bool if_senate_role = await user.HasDiscordRoleAsync(senate_role);

                if (data.district.ToLower() == ConfigJson.DistrictName.ToLower())
                {
                    await e.Member.GrantRoleAsync(district_role).ConfigureAwait(false);
                    await e.Member.GrantRoleAsync(unpicked_state_role).ConfigureAwait(false);
                }
                else
                {
                    await e.Member.GrantRoleAsync(non_citizen_role).ConfigureAwait(false);
                }
                if (if_senate_role == true)
                {
                    DiscordRole senator_role_id = e.Guild.GetRole(ConfigJson.SenateID);

                    await e.Member.GrantRoleAsync(senator_role_id).ConfigureAwait(false);
                }
                DiscordChannel welcome = e.Guild.GetChannel(ConfigJson.WelcomeID);
                await welcome.SendMessageAsync($"Welcome {e.Member.Mention} to {e.Guild.Name}!");
            }

            if (e.Member.Id == 470203136771096596)
            {
                await e.Member.BanAsync(0, "being Asdia");
            }
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
