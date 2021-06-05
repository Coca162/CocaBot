using DSharpPlus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DSharpPlus.CommandsNext;

namespace Discord
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public async Task RunAsync(DiscordConfig ConfigJson)
        {
            DiscordConfiguration config = new DiscordConfiguration
            {
                Token = ConfigJson.Token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady; ;

            CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = ConfigJson.Prefix
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            //Commands here

            Commands.RegisterCommands<Economy>();

            Commands.RegisterCommands<Misc>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
