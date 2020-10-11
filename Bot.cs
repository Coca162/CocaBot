using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.EventArgs;
using DSharpPlus.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SpookVooper.Api;
using CocaBot.Commands;

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

            var ConfigJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = ConfigJson.Token,
                TokenType = TokenType.Bot ,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };
                
            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            Client.GuildMemberAdded += Client_GuildMemberAdded;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] {ConfigJson.Prefix},
                EnableDms = false,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<ping>();

            Commands.RegisterCommands<statistics>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private async Task Client_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            var ServerID = e.Member.Guild.Id;
            if (ServerID == 762075097422495784)
            {
                var discordID = e.Member.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                var Data = await SpookVooperAPI.Users.GetUser(SVID);
                var new_yam_role = e.Member.Guild.GetRole(762434338847195138);
                var non_citizen_role = e.Member.Guild.GetRole(762739003630944296);

                if (Data.district == "New Yam")
                {

                    await e.Member.GrantRoleAsync(new_yam_role).ConfigureAwait(false);
                    await e.Member.RevokeRoleAsync(non_citizen_role).ConfigureAwait(false);
                }
                else
                {

                    await e.Member.GrantRoleAsync(non_citizen_role).ConfigureAwait(false);
                    await e.Member.RevokeRoleAsync(new_yam_role).ConfigureAwait(false);
                }
            }
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
