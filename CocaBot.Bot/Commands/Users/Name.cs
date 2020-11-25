using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api;
using System.Threading.Tasks;

namespace CocaBot.Commands
{
    public class Name : BaseCommandModule
    {
        [Command("name"), EnableBlacklist]
        [Aliases("nam", "n")]
        [Priority(0)]
        public async Task NameUser(CommandContext ctx, DiscordUser discordUser)
        {
            ulong discordID = discordUser.Id;
            string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
            string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);

            await ctx.Channel.SendMessageAsync($"{discordUser.Username}'s SV Name: {SV_Name}").ConfigureAwait(false);
        }

        [Command("name")]
        [Priority(1)]
        public async Task NameAll(CommandContext ctx, [RemainingText] string Inputname)
        {
            if (Inputname != null)
            {
                string gname = await SpookVooperAPI.Groups.GetName(Inputname);
                string uname = await SpookVooperAPI.Users.GetUsername(Inputname);

                if (gname == null && uname == null)
                {
                    await ctx.Channel.SendMessageAsync($"{Inputname} is not a SVID for a user or a group!").ConfigureAwait(false);
                }
                else
                {
                    await ctx.Channel.SendMessageAsync($"SVID: {gname}{uname}").ConfigureAwait(false);
                }
            }
            else
            {
                ulong discordID = ctx.Member.Id;
                string SVID = await SpookVooperAPI.Users.GetSVIDFromDiscord(discordID);
                string SV_Name = await SpookVooperAPI.Users.GetUsername(SVID);

                await ctx.Channel.SendMessageAsync($"{ctx.Member.Username}'s SV Name: {SV_Name}").ConfigureAwait(false);
            }
        }
    }
}

