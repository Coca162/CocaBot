using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Valour.Net;
using System.IO;
using System.Text.Json;
using Valour.Net.CommandHandling;
using Valour.Net.CommandHandling.Attributes;
using Valour.Net.Models;
using System.Linq;
using System.Collections.Generic;
using static Shared.Main;

namespace Valour
{
    class Program
    {
        public static ProfanityFilter.ProfanityFilter Filter = new ProfanityFilter.ProfanityFilter();

        static async Task Main()
        {
            string[] profanities = { "cocka", "discord", "dickord", "lolipop" };
            Filter.AddProfanity(profanities);

            ValourConfig config = await GetConfig<ValourConfig>();
            await BeginCocaBot(config, Platform.Discord);

            ValourClient.BotPrefixList = config.Prefix;

            await ValourClient.Start(config.Email, config.BotPassword);

            ValourClient.RegisterModules();

            //ValourClient.OnMessage += OnMessage;

            await Task.Delay(-1);
        }

        //public static async Task OnMessage(PlanetMessage message)
        //{
        //    if (message.Author.Nickname == "Allegate")
        //    {
        //        ValourClient.PostMessage(message.Channel_Id, message.Planet_Id, "yeah okay");
        //    }
        //}
    }
}
