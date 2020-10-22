using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
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
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            Client.GuildMemberAdded += Client_GuildMemberAdded;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { ConfigJson.Prefix },
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
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var ConfigJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var GuildID = e.Member.Guild.Id;
            if (GuildID == ConfigJson.ServerID)
            {
                var discordID = e.Member.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                var Data = await SpookVooperAPI.Users.GetUser(SVID);
                var district_role = e.Member.Guild.GetRole(ConfigJson.CitizenID);
                var non_citizen_role = e.Member.Guild.GetRole(ConfigJson.NonCitizenID);
                var senate_role = "Senator";
                string if_senate_role = await SpookVooperAPI.Users.HasDiscordRole(SVID, senate_role);

                if (Data.district == ConfigJson.DistrictName)
                {
                    await e.Member.GrantRoleAsync(district_role).ConfigureAwait(false);
                }
                else
                {
                    await e.Member.GrantRoleAsync(non_citizen_role).ConfigureAwait(false);
                }
                if (if_senate_role == "true")
                {
                    var senator_role_id = e.Guild.GetRole(ConfigJson.SenateID);

                    await e.Member.GrantRoleAsync(senator_role_id).ConfigureAwait(false);
                }
                var welcome = e.Guild.GetChannel(ConfigJson.SenateID);
                await welcome.SendMessageAsync($"Welcome {e.Member.Mention} to {e.Guild.Name}!");
            }
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
