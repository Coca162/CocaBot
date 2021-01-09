using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SpookVooper.Api.Entities;
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
            await ctx.Channel.SendMessageAsync(discordID.ToString()).ConfigureAwait(false);
            string svid = await User.GetSVIDFromDiscordAsync(discordID);
            User user = new User(svid);

            await ctx.RespondAsync($"{discordUser.Username}'s SV Name: {await user.GetUsernameAsync()}").ConfigureAwait(false);
        }

        [Command("name")]
        [Priority(1)]
        public async Task NameAll(CommandContext ctx, [RemainingText] string InputSVID)
        {
            if (InputSVID != null)
            {
                await ctx.RespondAsync($"Name: {await SVTools.SVIDToName(InputSVID)}").ConfigureAwait(false);
            }
            else
            {
                ulong discordID = ctx.Member.Id;
                User user = new User(await User.GetSVIDFromDiscordAsync(discordID));

                await ctx.RespondAsync($"{ctx.Member.Username}'s SV Name: {await user.GetUsernameAsync()}").ConfigureAwait(false);
            }
        }
    }
}

