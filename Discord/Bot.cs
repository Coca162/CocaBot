﻿using DSharpPlus;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Discord.Commands;
using Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using static Discord.Program;

namespace Discord
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public Bot(IServiceProvider services)
        {
            DiscordConfiguration discordConfig = new()
            {
                Token = Config.Token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug
            };

            Client = new DiscordClient(discordConfig);

            Client.Ready += OnClientReady; ;

            CommandsNextConfiguration commandsConfig = new()
            {
                StringPrefixes = Config.Prefix,
                Services = services
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            //Commands here

            Commands.RegisterCommands<Economy>();

            Commands.RegisterCommands<Misc>();

            Commands.RegisterCommands<Valour>();

            Commands.RegisterCommands<Stats>();

            Client.ConnectAsync();
        }

        private Task OnClientReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e) => Task.CompletedTask;
    }
}
