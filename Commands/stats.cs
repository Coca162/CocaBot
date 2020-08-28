using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SpookVooper.Api;
using System;
using System.Threading.Tasks;

namespace CocaBot.Commands
{
    public class statistics : BaseCommandModule
    {
        [Command("stats")]
        public async Task Statistics(CommandContext ctx)
        {
            var discordID = ctx.Member.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            var ISVID = SVID.ToString();

            await ctx.Channel
                .SendMessageAsync(ISVID.ToString())
                .ConfigureAwait(false);
        }
        
    }
}
