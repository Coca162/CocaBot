using System.Net.Http;
using System.Threading.Tasks;
using Valour.Net;
using static Shared.Main;

namespace Valour
{
    class Program
    {
        public static ProfanityFilter.ProfanityFilter Filter = new();
        static async Task Main()
        {
            string[] profanities = { "cocka", "discord", "dickord", "lolipop" };
            Filter.AddProfanity(profanities);

            ValourConfig config = await GetConfig<ValourConfig>();
            platform = Platform.Valour;
            await BeginCocaBot(config);

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
