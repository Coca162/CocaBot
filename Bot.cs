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

            ConfigJson ConfigJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            DiscordConfiguration config = new DiscordConfiguration
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

            CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration
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
            string json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            ConfigJson ConfigJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            ulong GuildID = e.Member.Guild.Id;
            if (GuildID == ConfigJson.ServerID)
            {
                ulong discordID = e.Member.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                SpookVooper.Api.Entities.User Data = await SpookVooperAPI.Users.GetUser(SVID);
                DSharpPlus.Entities.DiscordRole district_role = e.Member.Guild.GetRole(ConfigJson.CitizenID);
                DSharpPlus.Entities.DiscordRole non_citizen_role = e.Member.Guild.GetRole(ConfigJson.NonCitizenID);
                string senate_role = "Senator";
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
                    DSharpPlus.Entities.DiscordRole senator_role_id = e.Guild.GetRole(ConfigJson.SenateID);

                    await e.Member.GrantRoleAsync(senator_role_id).ConfigureAwait(false);
                }
                DSharpPlus.Entities.DiscordChannel welcome = e.Guild.GetChannel(ConfigJson.WelcomeID);
                await welcome.SendMessageAsync($"Welcome {e.Member.Mention} to {e.Guild.Name}!");
            }
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
