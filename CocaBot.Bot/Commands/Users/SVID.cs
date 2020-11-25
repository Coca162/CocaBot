using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api;
using System.Threading.Tasks;

namespace CocaBot.Commands
{
    public class SVID : BaseCommandModule
    {
        [Command("svid"), EnableBlacklist]
        [Aliases("s", "sv")]
        [Priority(0)]
        public async Task SVIDUser(CommandContext ctx, DiscordUser discordUser)
        {
            ulong discordID = discordUser.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);

            await ctx.Channel.SendMessageAsync($"{SV_Name}'s SVID: {SVID}").ConfigureAwait(false);
        }

        [Command("svid")]
        [Priority(1)]
        public async Task SVIDAll(CommandContext ctx, [RemainingText] string Inputname)
        {
            if (Inputname != null)
            {
                string gSVID = await SpookVooperAPI.Groups.GetSVIDFromName(Inputname);
                string uSVID = await SpookVooperAPI.Users.GetSVIDFromUsername(Inputname);

                if (uSVID == null && gSVID == null)
                {
                    await ctx.Channel.SendMessageAsync($"{Inputname} is not a user or a group!").ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendMessageAsync($"SVID: {gSVID}{uSVID}").ConfigureAwait(false);
                }
            }
            else
            {
                ulong discordID = ctx.Member.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);

                await ctx.Channel.SendMessageAsync($"{SV_Name}'s SVID: {SVID}").ConfigureAwait(false);
            }
        }
    }
}

